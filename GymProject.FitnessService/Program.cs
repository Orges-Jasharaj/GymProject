
using GymProject.Data;
using GymProject.Dtos.System;
using GymProject.Middleware;
using GymProject.Models;
using GymProject.Repositories.Implemntations;
using GymProject.Repositories.Interfaces;

using GymProject.Services.Implementation;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;

using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace GymProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.




            builder.Services.AddHttpContextAccessor();


            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);

            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter  your token in the text input below.\n\nExample: '12345abcdef'",
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
             {
                 {
                     new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                     {
                         Reference = new Microsoft.OpenApi.Models.OpenApiReference
                         {
                             Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                     },
                     new string[] { }
                 }
             });
            });

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            builder.Services.AddScoped<DapperContext>();

            builder.Services.AddScoped<CurrentUserService>();
            builder.Services.AddHttpClient<IUserProfileClient, UserProfileClient>(client =>
            {
                var identityUrl = builder.Configuration["IdentityServiceUrl"] ?? "https://localhost:7092/";
                client.BaseAddress = new Uri(identityUrl);
            });
            builder.Services.AddScoped<IFitnessPlansRepository, FitnessPlansRepository>();
            builder.Services.AddScoped<IFitnessPlanService, FitnessPlanService>();
            builder.Services.AddScoped<IExercisesRepository, ExercisesRepository>();
            builder.Services.AddScoped<IExercisesService, ExercisesService>();
            builder.Services.AddScoped<IPlanExercisesRepository, PlanExercisesRepository>();
            builder.Services.AddScoped<IPlanExercisesService, PlanExercisesService>();

            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(app =>
                {
                    app.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // Initialize database and tables (Dapper has no auto-migration)
            await InitializeDatabaseAsync(app.Configuration);

            app.Run();
        }

        private static async Task InitializeDatabaseAsync(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            builder.InitialCatalog = "master";
            using (var masterConnection = new SqlConnection(builder.ConnectionString))
            {
                await masterConnection.OpenAsync();
                using var checkCmd = masterConnection.CreateCommand();
                checkCmd.CommandText = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
                    BEGIN
                        CREATE DATABASE [{databaseName}];
                    END";
                await checkCmd.ExecuteNonQueryAsync();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Exercises')
                    BEGIN
                        CREATE TABLE Exercises (
                            Id UNIQUEIDENTIFIER PRIMARY KEY,
                            Name NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(MAX) NULL,
                            MuscleGroup NVARCHAR(100) NULL,
                            Equipment NVARCHAR(100) NULL,
                            CreatedBy NVARCHAR(200) NOT NULL,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            UpdatedBy NVARCHAR(200) NULL,
                            UpdatedAt DATETIME2 NULL
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FitnessPlans')
                    BEGIN
                        CREATE TABLE FitnessPlans (
                            Id UNIQUEIDENTIFIER PRIMARY KEY,
                            UserId UNIQUEIDENTIFIER NOT NULL,
                            Name NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(MAX) NULL,
                            CreatedBy NVARCHAR(200) NOT NULL,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            UpdatedBy NVARCHAR(200) NULL,
                            UpdatedAt DATETIME2 NULL
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PlanExercises')
                    BEGIN
                        CREATE TABLE PlanExercises (
                            Id UNIQUEIDENTIFIER PRIMARY KEY,
                            FitnessPlanId UNIQUEIDENTIFIER NOT NULL,
                            ExerciseId UNIQUEIDENTIFIER NOT NULL,
                            Sets INT NOT NULL,
                            Reps INT NOT NULL,
                            ExerciseOrder INT NOT NULL,
                            CreatedBy NVARCHAR(200) NOT NULL,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            UpdatedBy NVARCHAR(200) NULL,
                            UpdatedAt DATETIME2 NULL,
                            FOREIGN KEY (FitnessPlanId) REFERENCES FitnessPlans(Id),
                            FOREIGN KEY (ExerciseId) REFERENCES Exercises(Id)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
                    BEGIN
                        CREATE TABLE AuditLogs (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId NVARCHAR(200) NULL,
                            Type NVARCHAR(50) NOT NULL,
                            TableName NVARCHAR(200) NOT NULL,
                            DateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            OldValues NVARCHAR(MAX) NULL,
                            NewValues NVARCHAR(MAX) NULL,
                            AffectedColumns NVARCHAR(MAX) NULL,
                            PrimaryKey NVARCHAR(200) NOT NULL
                        );
                    END";
                await cmd.ExecuteNonQueryAsync();
            }

            Log.Information("Database '{DatabaseName}' initialized successfully", databaseName);
        }
    }
}
