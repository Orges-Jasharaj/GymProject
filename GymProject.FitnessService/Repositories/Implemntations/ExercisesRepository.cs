using Dapper;
using GymProject.Data;
using GymProject.Models;
using GymProject.Repositories.Interfaces;

namespace GymProject.Repositories.Implemntations
{
    public class ExercisesRepository : IExercisesRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ExercisesRepository> _logger;

        public ExercisesRepository(DapperContext context, ILogger<ExercisesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<bool> CreateExercise(Exercises exercise)
        {
            try
            {
                var query = @"INSERT INTO Exercises
                            (Id, Name, Description, MuscleGroup, Equipment, CreatedBy, CreatedAt)
                            VALUES
                            (@Id, @Name, @Description, @MuscleGroup, @Equipment, @CreatedBy, @CreatedAt)";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, exercise) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise");
                return false;
            }
        }

        public async Task<bool> DeleteExercise(Guid id)
        {
            try
            {
                var query = "DELETE FROM Exercises WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new { Id = id }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise with Id: {Id}", id);
                return false;
            }
        }

        public async Task<List<Exercises>> GetAllExercises()
        {
            try
            {
                var query = "SELECT * FROM Exercises";
                using var connection = _context.CreateConnection();
                var exercises = await connection.QueryAsync<Exercises>(query);
                return exercises.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercises");
                return new List<Exercises>();
            }
        }

        public async Task<Exercises> GetExerciseById(Guid id)
        {
            try
            {
                var query = "SELECT * FROM Exercises WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<Exercises>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise with Id: {Id}", id);
                return null;
            }
        }

        public async Task<bool> UpdateExercise(Guid id, Exercises exercise)
        {
            try
            {
                var query = @"UPDATE Exercises
                            SET Name = @Name,
                                Description = @Description,
                                MuscleGroup = @MuscleGroup,
                                Equipment = @Equipment,
                                UpdatedBy = @UpdatedBy,
                                UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    exercise.Name,
                    exercise.Description,
                    exercise.MuscleGroup,
                    exercise.Equipment,
                    exercise.UpdatedBy,
                    exercise.UpdatedAt
                }) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise with Id: {Id}", id);
                return false;
            }
        }
    }
}
