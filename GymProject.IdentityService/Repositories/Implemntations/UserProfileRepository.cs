using Dapper;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GymProject.Repositories.Implemntations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserProfileRepository> _logger;

        public UserProfileRepository(IConfiguration configuration, ILogger<UserProfileRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            try
            {
                var sql = @"
                    SELECT Id, UserId, Gender, HeightCm, CurrentWeightKg, GoalWeightKg,
                           ActivityLevel, FitnessGoal, DateOfBirth, CreatedAt, UpdatedAt
                    FROM UserProfiles
                    WHERE UserId = @UserId";

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { UserId = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> CreateAsync(UserProfile userProfile)
        {
            try
            {
                var sql = @"
                    INSERT INTO UserProfiles
                    (Id, UserId, Gender, HeightCm, CurrentWeightKg, GoalWeightKg, ActivityLevel, FitnessGoal, DateOfBirth, CreatedAt, UpdatedAt)
                    VALUES
                    (@Id, @UserId, @Gender, @HeightCm, @CurrentWeightKg, @GoalWeightKg, @ActivityLevel, @FitnessGoal, @DateOfBirth, @CreatedAt, @UpdatedAt)";

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return await connection.ExecuteAsync(sql, userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for UserId: {UserId}", userProfile.UserId);
                throw;
            }
        }

        public async Task<int> UpdateAsync(UserProfile userProfile)
        {
            try
            {
                var sql = @"
                    UPDATE UserProfiles
                    SET Gender = @Gender,
                        HeightCm = @HeightCm,
                        CurrentWeightKg = @CurrentWeightKg,
                        GoalWeightKg = @GoalWeightKg,
                        ActivityLevel = @ActivityLevel,
                        FitnessGoal = @FitnessGoal,
                        DateOfBirth = @DateOfBirth,
                        UpdatedAt = @UpdatedAt
                    WHERE UserId = @UserId";

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return await connection.ExecuteAsync(sql, userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for UserId: {UserId}", userProfile.UserId);
                throw;
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var sql = "DELETE FROM UserProfiles WHERE Id = @Id";

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                return await connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile with Id: {ProfileId}", id);
                throw;
            }
        }

        public async Task<Guid?> GetUserProfileIdByUserId(string userId)
        {
            var query = "SELECT Id FROM UserProfiles WHERE UserId = @UserId";

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryFirstOrDefaultAsync<Guid?>(query, new { UserId = userId });
        }
    }
}