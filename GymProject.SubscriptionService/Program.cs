using GymProject.Dtos.System;
using GymProject.Repositories.Implemntations;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Implementation;
using GymProject.Services.Interface;
using GymProject.SubscriptionService.Data;
using GymProject.SubscriptionService.Services.Implementation;
using GymProject.SubscriptionService.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using IUserProfileClient = GymProject.SubscriptionService.Services.Interface.IUserProfileClient;
using UserProfileClient = GymProject.SubscriptionService.Services.Implementation.UserProfileClient;

namespace GymProject.SubscriptionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<EntityContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

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

            builder.Services.AddScoped<CurrentUserService>();
            builder.Services.AddHttpClient<IUserProfileClient, UserProfileClient>(client =>
            {
                var identityUrl = builder.Configuration["IdentityServiceUrl"] ?? "https://localhost:7092/";
                client.BaseAddress = new Uri(identityUrl);
            });


            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<DapperContext>();

            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();

            builder.Services.AddScoped<IGymService, GymService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(app =>
                {
                    app.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
