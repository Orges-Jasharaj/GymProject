using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;

namespace GymProject.Services.Implementation
{
    public class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _exercisesRepository;
        private readonly ILogger<ExercisesService> _logger;
        private readonly CurrentUserService _currentUserService;
        private readonly IAuditLogService _auditLogService;

        public ExercisesService(IExercisesRepository exercisesRepository, ILogger<ExercisesService> logger, CurrentUserService currentUserService, IAuditLogService auditLogService)
        {
            _exercisesRepository = exercisesRepository;
            _logger = logger;
            _currentUserService = currentUserService;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto<ExercisesDto>> CreateExercise(CreateExercisesDto exercise)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var newExercise = new Exercises
                {
                    Id = Guid.NewGuid(),
                    Name = exercise.Name,
                    Description = exercise.Description,
                    MuscleGroup = exercise.MuscleGroup,
                    Equipment = exercise.Equipment,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _exercisesRepository.CreateExercise(newExercise);

                if (created)
                {
                    await _auditLogService.LogActivityAsync<Exercises>(currentUserId, "Create", "Exercises", newExercise.Id.ToString(), null, newExercise);
                    var createdDto = new ExercisesDto
                    {
                        Id = newExercise.Id,
                        Name = newExercise.Name,
                        Description = newExercise.Description,
                        MuscleGroup = newExercise.MuscleGroup,
                        Equipment = newExercise.Equipment,
                        CreatedBy = newExercise.CreatedBy,
                        CreatedAt = newExercise.CreatedAt,
                        UpdatedBy = newExercise.UpdatedBy,
                        UpdatedAt = newExercise.UpdatedAt
                    };
                    return ResponseDto<ExercisesDto>.SuccessResponse(createdDto, "Exercise created successfully.");
                }

                _logger.LogWarning("Failed to create exercise for user {UserId}", currentUserId);
                return ResponseDto<ExercisesDto>.Failure("Failed to create the exercise.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise");
                return ResponseDto<ExercisesDto>.Failure("An error occurred while creating the exercise.");
            }
        }

        public async Task<ResponseDto<bool>> DeleteExercise(Guid id)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var existingExercise = await _exercisesRepository.GetExerciseById(id);
                if (existingExercise == null)
                {
                    _logger.LogWarning("Exercise with id {ExerciseId} not found for user {UserId}", id, currentUserId);
                    return ResponseDto<bool>.Failure("Exercise not found.");
                }
                if (existingExercise.CreatedBy != currentUserId)
                {
                    _logger.LogWarning("User {UserId} attempted to delete exercise with id {ExerciseId} created by another user", currentUserId, id);
                    return ResponseDto<bool>.Failure("You do not have permission to delete this exercise.");
                }
                var deleted = await _exercisesRepository.DeleteExercise(id);
                
                if (deleted)
                {
                    await _auditLogService.LogActivityAsync<Exercises>(currentUserId, "Delete", "Exercises", id.ToString(), existingExercise, null);
                }

                if (!deleted)
                {
                    _logger.LogWarning("Failed to delete exercise with id {ExerciseId} for user {UserId}", id, currentUserId);
                    return ResponseDto<bool>.Failure("Failed to delete the exercise.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise with id {ExerciseId}", id);
                return ResponseDto<bool>.Failure("An error occurred while deleting the exercise.");
            }
             return ResponseDto<bool>.SuccessResponse(true, "Exercise deleted successfully.");
        }

        public async Task<ResponseDto<List<ExercisesDto>>> GetAllExercises()
        {
            try
            {
                var exercises = await _exercisesRepository.GetAllExercises();
                var exercisesDto = exercises.Select(e => new ExercisesDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    MuscleGroup = e.MuscleGroup,
                    Equipment = e.Equipment,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedAt,
                    UpdatedBy = e.UpdatedBy,
                    UpdatedAt = e.UpdatedAt
                }).ToList();
                return ResponseDto<List<ExercisesDto>>.SuccessResponse(exercisesDto, "Exercises retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercises");
                return ResponseDto<List<ExercisesDto>>.Failure("An error occurred while retrieving exercises.");
            }
        }

        public async Task<ResponseDto<List<ExercisesDto>>> GetExercisesByMuscleGroup(string muscleGroup)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(muscleGroup))
                {
                    return ResponseDto<List<ExercisesDto>>.Failure("Muscle group is required.");
                }

                var exercises = await _exercisesRepository.GetExercisesByMuscleGroup(muscleGroup);
                var exercisesDto = exercises.Select(e => new ExercisesDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    MuscleGroup = e.MuscleGroup,
                    Equipment = e.Equipment,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedAt,
                    UpdatedBy = e.UpdatedBy,
                    UpdatedAt = e.UpdatedAt
                }).ToList();

                return ResponseDto<List<ExercisesDto>>.SuccessResponse(exercisesDto, "Exercises retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercises for muscle group {MuscleGroup}", muscleGroup);
                return ResponseDto<List<ExercisesDto>>.Failure("An error occurred while retrieving exercises.");
            }
        }

        public async Task<ResponseDto<ExercisesDto>> GetExerciseById(Guid id)
        {
            try
            {
                var exercise = await _exercisesRepository.GetExerciseById(id);
                if (exercise != null)
                {
                    var exerciseDto = new ExercisesDto
                    {
                        Id = exercise.Id,
                        Name = exercise.Name,
                        Description = exercise.Description,
                        MuscleGroup = exercise.MuscleGroup,
                        Equipment = exercise.Equipment,
                        CreatedBy = exercise.CreatedBy,
                        CreatedAt = exercise.CreatedAt,
                        UpdatedBy = exercise.UpdatedBy,
                        UpdatedAt = exercise.UpdatedAt
                    };
                    return ResponseDto<ExercisesDto>.SuccessResponse(exerciseDto, "Exercise retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise with id {ExerciseId}", id);
                return ResponseDto<ExercisesDto>.Failure("An error occurred while retrieving the exercise.");
            }
             return ResponseDto<ExercisesDto>.Failure("Exercise not found.");
        }

        public async Task<ResponseDto<bool>> UpdateExercise(Guid id, CreateExercisesDto exercise)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var existingExercise = await _exercisesRepository.GetExerciseById(id);
                if (existingExercise == null)
                {
                    _logger.LogWarning("Exercise with id {ExerciseId} not found for user {UserId}", id, currentUserId);
                    return ResponseDto<bool>.Failure("Exercise not found.");
                }
                if (existingExercise.CreatedBy != currentUserId)
                {
                    _logger.LogWarning("User {UserId} attempted to update exercise with id {ExerciseId} created by another user", currentUserId, id);
                    return ResponseDto<bool>.Failure("You do not have permission to update this exercise.");
                }
                var oldExercise = new Exercises
                {
                    Id = existingExercise.Id,
                    Name = existingExercise.Name,
                    Description = existingExercise.Description,
                    MuscleGroup = existingExercise.MuscleGroup,
                    Equipment = existingExercise.Equipment,
                    CreatedBy = existingExercise.CreatedBy,
                    CreatedAt = existingExercise.CreatedAt,
                    UpdatedBy = existingExercise.UpdatedBy,
                    UpdatedAt = existingExercise.UpdatedAt
                };

                existingExercise.Name = exercise.Name;
                existingExercise.Description = exercise.Description;
                existingExercise.MuscleGroup = exercise.MuscleGroup;
                existingExercise.Equipment = exercise.Equipment;
                existingExercise.UpdatedBy = currentUserId;
                existingExercise.UpdatedAt = DateTime.UtcNow;
                var updated = await _exercisesRepository.UpdateExercise(id,existingExercise);
                
                if (updated)
                {
                    await _auditLogService.LogActivityAsync<Exercises>(currentUserId, "Update", "Exercises", id.ToString(), oldExercise, existingExercise);
                }

                if (!updated)
                {
                    _logger.LogWarning("Failed to update exercise with id {ExerciseId} for user {UserId}", id, currentUserId);
                    return ResponseDto<bool>.Failure("Failed to update the exercise.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise with id {ExerciseId}", id);
                return ResponseDto<bool>.Failure("An error occurred while updating the exercise.");
            }
            return ResponseDto<bool>.SuccessResponse(true, "Exercise updated successfully." );
        }
    }
}
