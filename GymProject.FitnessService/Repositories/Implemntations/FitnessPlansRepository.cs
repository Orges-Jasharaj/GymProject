using Dapper;
using GymProject.Data;
using GymProject.Models;
using GymProject.Repositories.Interfaces;

namespace GymProject.Repositories.Implemntations
{
    public class FitnessPlansRepository : IFitnessPlansRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<FitnessPlansRepository> _logger;

        public FitnessPlansRepository(DapperContext context, ILogger<FitnessPlansRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateFitnessPlan(FitnessPlans fitnessPlan)
        {
            try
            {
                var query = @"INSERT INTO FitnessPlans 
                            (Id, UserId, Name, Description, CreatedBy, CreatedAt) 
                             VALUES 
                            (@Id, @UserId, @Name, @Description, @CreatedBy, @CreatedAt)";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, fitnessPlan) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fitness plan");
                return false;
            }
        }

        public async Task<bool> DeleteFitnessPlan(Guid id)
        {
            try
            {
                var query = "DELETE FROM FitnessPlans WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new { Id = id }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fitness plan with Id: {Id}", id);
                return false;
            }
        }

        public async Task<List<FitnessPlans>> GetAllFitnessPlans(Guid userId)
        {
            try
            {
                var query = "SELECT * FROM FitnessPlans WHERE UserId = @UserId";

                using var connection = _context.CreateConnection();
                var fitnessPlans = await connection.QueryAsync<FitnessPlans>(query, new { UserId = userId });

                return fitnessPlans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fitness plans");
                return new List<FitnessPlans>();
            }
        }

        public async Task<FitnessPlans?> GetFitnessPlanById(Guid id)
        {
            try
            {
                var query = "SELECT * FROM FitnessPlans WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<FitnessPlans>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fitness plan with Id: {Id}", id);
                return null;
            }
        }

        public async Task<bool> UpdateFitnessPlan(FitnessPlans fitnessPlan)
        {
            try
            {
                var query = @"UPDATE FitnessPlans 
                              SET Name = @Name, 
                                  Description = @Description,
                                  UpdatedBy = @UpdatedBy,
                                  UpdatedAt = @UpdatedAt
                              WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, fitnessPlan) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fitness plan with Id: {Id}", fitnessPlan.Id);
                return false;
            }
        }
    }
}