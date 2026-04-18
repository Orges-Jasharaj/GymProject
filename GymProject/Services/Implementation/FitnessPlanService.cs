using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;

namespace GymProject.Services.Implementation
{
    public class FitnessPlanService : IFitnessPlanService
    {
        private readonly ILogger<FitnessPlanService> _logger;
        private readonly CurrentUserService _currentUserService;
        private readonly IFitnessPlansRepository _fitnessPlansRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAuditLogService _auditLogService;

        public FitnessPlanService(
            ILogger<FitnessPlanService> logger,
            CurrentUserService currentUserService,
            IFitnessPlansRepository fitnessPlansRepository,
            IUserProfileRepository userProfileRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _fitnessPlansRepository = fitnessPlansRepository;
            _userProfileRepository = userProfileRepository;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto<bool>> CreateFitnessPlanAsync(CreateFitnessPlansDto fitnessPlanDto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var userProfileId = await _userProfileRepository.GetUserProfileIdByUserId(currentUserId);

                if (userProfileId == null)
                    return ResponseDto<bool>.Failure("User profile not found");

                var fitnessPlan = new FitnessPlans
                {
                    Id = Guid.NewGuid(),
                    UserId = userProfileId.Value,
                    Name = fitnessPlanDto.Name,
                    Description = fitnessPlanDto.Description,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _fitnessPlansRepository.CreateFitnessPlan(fitnessPlan);

                if (created)
                {
                    await _auditLogService.LogActivityAsync<FitnessPlans>(currentUserId, "Create", "FitnessPlans", fitnessPlan.Id.ToString(), null, fitnessPlan);
                }

                if (!created)
                    return ResponseDto<bool>.Failure("Failed to create fitness plan");

                return ResponseDto<bool>.SuccessResponse(true, "Fitness plan created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fitness plan");
                return ResponseDto<bool>.Failure("An error occurred while creating the fitness plan.");
            }
        }

        public async Task<ResponseDto<List<FitnessPlansDto>>> GetAllFitnessPlansAsync()
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var userProfileId = await _userProfileRepository.GetUserProfileIdByUserId(currentUserId);

                if (userProfileId == null)
                    return ResponseDto<List<FitnessPlansDto>>.Failure("User profile not found");

                var fitnessPlans = await _fitnessPlansRepository.GetAllFitnessPlans(userProfileId.Value);

                var result = fitnessPlans.Select(fp => new FitnessPlansDto
                {
                    Id = fp.Id,
                    UserId = fp.UserId,
                    Name = fp.Name,
                    Description = fp.Description,
                    CreatedBy = fp.CreatedBy,
                    CreatedAt = fp.CreatedAt,
                    UpdatedBy = fp.UpdatedBy,
                    UpdatedAt = fp.UpdatedAt
                }).ToList();

                return ResponseDto<List<FitnessPlansDto>>.SuccessResponse(result, "Fitness plans retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fitness plans");
                return ResponseDto<List<FitnessPlansDto>>.Failure("An error occurred while retrieving fitness plans.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateFitnessPlanAsync(Guid id, CreateFitnessPlansDto dto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                var fitnessPlan = await _fitnessPlansRepository.GetFitnessPlanById(id);

                if (fitnessPlan == null)
                    return ResponseDto<bool>.Failure("Fitness plan not found");
                    
                var oldFitnessPlan = new FitnessPlans
                {
                    Id = fitnessPlan.Id,
                    UserId = fitnessPlan.UserId,
                    Name = fitnessPlan.Name,
                    Description = fitnessPlan.Description,
                    CreatedBy = fitnessPlan.CreatedBy,
                    CreatedAt = fitnessPlan.CreatedAt,
                    UpdatedBy = fitnessPlan.UpdatedBy,
                    UpdatedAt = fitnessPlan.UpdatedAt
                };

                fitnessPlan.Name = dto.Name;
                fitnessPlan.Description = dto.Description;
                fitnessPlan.UpdatedBy = currentUserId;
                fitnessPlan.UpdatedAt = DateTime.UtcNow;

                var updated = await _fitnessPlansRepository.UpdateFitnessPlan(fitnessPlan);

                if (updated)
                {
                    await _auditLogService.LogActivityAsync<FitnessPlans>(currentUserId, "Update", "FitnessPlans", id.ToString(), oldFitnessPlan, fitnessPlan);
                }

                if (!updated)
                    return ResponseDto<bool>.Failure("Failed to update fitness plan");

                return ResponseDto<bool>.SuccessResponse(true, "Fitness plan updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fitness plan");
                return ResponseDto<bool>.Failure("Error updating fitness plan");
            }
        }

        public async Task<ResponseDto<bool>> DeleteFitnessPlanAsync(Guid id)
        {
            try
            {
                var fitnessPlan = await _fitnessPlansRepository.GetFitnessPlanById(id);

                if (fitnessPlan == null)
                    return ResponseDto<bool>.Failure("Fitness plan not found");

                var deleted = await _fitnessPlansRepository.DeleteFitnessPlan(id);

                if (deleted)
                {
                    var currentUserId = _currentUserService.GetCurrentUserId();
                    await _auditLogService.LogActivityAsync<FitnessPlans>(currentUserId, "Delete", "FitnessPlans", id.ToString(), fitnessPlan, null);
                }

                if (!deleted)
                    return ResponseDto<bool>.Failure("Failed to delete fitness plan");

                return ResponseDto<bool>.SuccessResponse(true, "Deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fitness plan");
                return ResponseDto<bool>.Failure("Error deleting fitness plan");
            }
        }

        public async Task<ResponseDto<FitnessPlansDto>> GetFitnessPlanByIdAsync(Guid id)
        {
            try
            {
                var fitnessPlan = await _fitnessPlansRepository.GetFitnessPlanById(id);

                if (fitnessPlan == null)
                    return ResponseDto<FitnessPlansDto>.Failure("Fitness plan not found");

                var result = new FitnessPlansDto
                {
                    Id = fitnessPlan.Id,
                    UserId = fitnessPlan.UserId,
                    Name = fitnessPlan.Name,
                    Description = fitnessPlan.Description,
                    CreatedBy = fitnessPlan.CreatedBy,
                    CreatedAt = fitnessPlan.CreatedAt,
                    UpdatedBy = fitnessPlan.UpdatedBy,
                    UpdatedAt = fitnessPlan.UpdatedAt
                };

                return ResponseDto<FitnessPlansDto>.SuccessResponse(result, "Fitness plan retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fitness plan");
                return ResponseDto<FitnessPlansDto>.Failure("Error retrieving fitness plan");
            }
        }
    }
}