using Dapper;
using GymProject.Data;
using GymProject.Models;
using GymProject.Repositories.Interfaces;

namespace GymProject.Repositories.Implemntations
{
    public class PlanExercisesRepository : IPlanExercisesRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<PlanExercisesRepository> _logger;
        public PlanExercisesRepository(DapperContext context, ILogger<PlanExercisesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddPlanExerciseAsync(PlanExercises planExercise)
        {
            try
            {
                var query = @"INSERT INTO PlanExercises (Id, FitnessPlanId, ExerciseId, Sets, Reps, ExerciseOrder, CreatedBy, CreatedAt)
                              VALUES (@Id, @FitnessPlanId, @ExerciseId, @Sets, @Reps, @ExerciseOrder, @CreatedBy, @CreatedAt)";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, planExercise) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plan exercise");
                return false;
            }
        }

        public async Task<bool> DeletePlanExerciseAsync(Guid id)
        {
            try
            {
                var query = "DELETE FROM PlanExercises WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new { Id = id }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan exercise with Id: {Id}", id);
                return false;
            }
        }

        public async Task<List<PlanExercises>> GetAllPlanExercisesAsync()
        {
            try
            {
                var query = "SELECT * FROM PlanExercises";
                using var connection = _context.CreateConnection();
                var result = await connection.QueryAsync<PlanExercises>(query);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all plan exercises");
                return new List<PlanExercises>();
            }
        }

        public async Task<PlanExercises?> GetPlanExerciseByIdAsync(Guid id)
        {
            try
            {
                var query = "SELECT * FROM PlanExercises WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<PlanExercises>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plan exercise with Id: {Id}", id);
                return null;
            }
        }

        public async Task<bool> UpdatePlanExerciseAsync(Guid id,PlanExercises planExercise)
        {
            try
            {
                var query = @"UPDATE PlanExercises
                              SET FitnessPlanId = @FitnessPlanId, ExerciseId = @ExerciseId, Sets = @Sets, Reps = @Reps, ExerciseOrder = @ExerciseOrder, CreatedBy = @CreatedBy, CreatedAt = @CreatedAt
                              WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, planExercise) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan exercise with Id: {Id}", planExercise.Id);
                return false;
            }
        }
    }
}
