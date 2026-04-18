using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;

namespace GymProject.Services.Implementation
{
    public class PlanExercisesService : IPlanExercisesService
    {
        private readonly IPlanExercisesRepository _planExercisesRepository;
        private readonly ILogger<PlanExercisesService> _logger;
        private readonly CurrentUserService _currentUserService;

        public PlanExercisesService(
            IPlanExercisesRepository planExercisesRepository,
            ILogger<PlanExercisesService> logger,
            CurrentUserService currentUserService)
        {
            _planExercisesRepository = planExercisesRepository;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseDto<bool>> AddPlanExerciseAsync(CreatePlanExercisesDto planExercise)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var newPlanExercise = new PlanExercises
                {
                    Id = Guid.NewGuid(),
                    FitnessPlanId = planExercise.FitnessPlanId,
                    ExerciseId = planExercise.ExerciseId,
                    Sets = planExercise.Sets,
                    Reps = planExercise.Reps,
                    ExerciseOrder = planExercise.ExerciseOrder,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                };

                var created = await _planExercisesRepository.AddPlanExerciseAsync(newPlanExercise);

                if (!created)
                {
                    _logger.LogWarning("Failed to create plan exercise for user {UserId}", currentUserId);
                    return ResponseDto<bool>.Failure("Failed to create plan exercise.");
                }

                return ResponseDto<bool>.SuccessResponse(true, "Plan exercise created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding plan exercise");
                return ResponseDto<bool>.Failure("An error occurred while adding plan exercise.");
            }
        }

        public async Task<ResponseDto<bool>> DeletePlanExerciseAsync(Guid id)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var existing = await _planExercisesRepository.GetPlanExerciseByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Plan exercise {Id} not found", id);
                    return ResponseDto<bool>.Failure("Plan exercise not found.");
                }

                if (existing.CreatedBy != currentUserId)
                {
                    _logger.LogWarning("User {UserId} tried to delete unauthorized plan exercise {Id}", currentUserId, id);
                    return ResponseDto<bool>.Failure("You do not have permission to delete this item.");
                }

                var deleted = await _planExercisesRepository.DeletePlanExerciseAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Failed to delete plan exercise {Id}", id);
                    return ResponseDto<bool>.Failure("Failed to delete plan exercise.");
                }

                return ResponseDto<bool>.SuccessResponse(true, "Plan exercise deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan exercise {Id}", id);
                return ResponseDto<bool>.Failure("An error occurred while deleting plan exercise.");
            }
        }

        public async Task<ResponseDto<List<PlanExercisesDto>>> GetAllPlanExercisesAsync()
        {
            try
            {
                var data = await _planExercisesRepository.GetAllPlanExercisesAsync();

                var result = data.Select(x => new PlanExercisesDto
                {
                    Id = x.Id,
                    FitnessPlanId = x.FitnessPlanId,
                    ExerciseId = x.ExerciseId,
                    Sets = x.Sets,
                    Reps = x.Reps,
                    ExerciseOrder = x.ExerciseOrder,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy
                }).ToList();

                return ResponseDto<List<PlanExercisesDto>>
                    .SuccessResponse(result, "Plan exercises retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plan exercises");
                return ResponseDto<List<PlanExercisesDto>>
                    .Failure("An error occurred while retrieving plan exercises.");
            }
        }

        public async Task<ResponseDto<PlanExercisesDto>> GetPlanExerciseByIdAsync(Guid id)
        {
            try
            {
                var entity = await _planExercisesRepository.GetPlanExerciseByIdAsync(id);

                if (entity == null)
                    return ResponseDto<PlanExercisesDto>.Failure("Plan exercise not found.");

                var dto = new PlanExercisesDto
                {
                    Id = entity.Id,
                    FitnessPlanId = entity.FitnessPlanId,
                    ExerciseId = entity.ExerciseId,
                    Sets = entity.Sets,
                    Reps = entity.Reps,
                    ExerciseOrder = entity.ExerciseOrder,
                    CreatedAt = entity.CreatedAt,
                    CreatedBy = entity.CreatedBy,
                    UpdatedAt = entity.UpdatedAt,
                    UpdatedBy = entity.UpdatedBy
                };

                return ResponseDto<PlanExercisesDto>
                    .SuccessResponse(dto, "Plan exercise retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plan exercise {Id}", id);
                return ResponseDto<PlanExercisesDto>
                    .Failure("An error occurred while retrieving plan exercise.");
            }
        }

        public async Task<ResponseDto<bool>> UpdatePlanExerciseAsync(Guid id, CreatePlanExercisesDto planExercise)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var existing = await _planExercisesRepository.GetPlanExerciseByIdAsync(id);
                if (existing == null)
                {
                    return ResponseDto<bool>.Failure("Plan exercise not found.");
                }

                if (existing.CreatedBy != currentUserId)
                {
                    return ResponseDto<bool>.Failure("You do not have permission to update this item.");
                }

                existing.FitnessPlanId = planExercise.FitnessPlanId;
                existing.ExerciseId = planExercise.ExerciseId;
                existing.Sets = planExercise.Sets;
                existing.Reps = planExercise.Reps;
                existing.ExerciseOrder = planExercise.ExerciseOrder;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = currentUserId;

                var updated = await _planExercisesRepository.UpdatePlanExerciseAsync(id, existing);

                if (!updated)
                {
                    return ResponseDto<bool>.Failure("Failed to update plan exercise.");
                }

                return ResponseDto<bool>.SuccessResponse(true, "Plan exercise updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan exercise {Id}", id);
                return ResponseDto<bool>.Failure("An error occurred while updating plan exercise.");
            }
        }
    }
}