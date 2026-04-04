using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;

namespace GymProject.Services.Implementation
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            CurrentUserService currentUserService,
            ILogger<UserProfileService> logger)
        {
            _userProfileRepository = userProfileRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResponseDto<UserProfileDto>> GetMyProfileAsync()
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    _logger.LogWarning("Unauthorized attempt to access user profile.");
                    return ResponseDto<UserProfileDto>.Failure("User not authenticated.");
                }

                var profile = await _userProfileRepository.GetByUserIdAsync(currentUserId);

                if (profile == null)
                {
                    _logger.LogInformation("User profile not found for UserId: {UserId}", currentUserId);
                    return ResponseDto<UserProfileDto>.Failure("Profile not found.");
                }

                var profileDto = new UserProfileDto
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    Gender = profile.Gender,
                    HeightCm = profile.HeightCm,
                    CurrentWeightKg = profile.CurrentWeightKg,
                    GoalWeightKg = profile.GoalWeightKg,
                    ActivityLevel = profile.ActivityLevel,
                    FitnessGoal = profile.FitnessGoal,
                    DateOfBirth = profile.DateOfBirth,
                    CreatedAt = profile.CreatedAt,
                    UpdatedAt = profile.UpdatedAt
                };

                _logger.LogInformation("User profile retrieved successfully for UserId: {UserId}", currentUserId);

                return ResponseDto<UserProfileDto>.SuccessResponse(profileDto, "Profile retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving current user profile.");
                return ResponseDto<UserProfileDto>.Failure("An error occurred while retrieving profile.");
            }
        }

        public async Task<ResponseDto<bool>> CreateAsync(CreateUserProfileRequest request)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    _logger.LogWarning("Unauthorized attempt to create user profile.");
                    return ResponseDto<bool>.Failure("User not authenticated.");
                }

                var existingProfile = await _userProfileRepository.GetByUserIdAsync(currentUserId);

                if (existingProfile != null)
                {
                    _logger.LogInformation("Attempt to create duplicate profile for UserId: {UserId}", currentUserId);
                    return ResponseDto<bool>.Failure("Profile already exists.");
                }

                var profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = currentUserId,
                    Gender = request.Gender,
                    HeightCm = request.HeightCm,
                    CurrentWeightKg = request.CurrentWeightKg,
                    GoalWeightKg = request.GoalWeightKg,
                    ActivityLevel = request.ActivityLevel,
                    FitnessGoal = request.FitnessGoal,
                    DateOfBirth = request.DateOfBirth,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userProfileRepository.CreateAsync(profile);

                if (result <= 0)
                {
                    _logger.LogWarning("Failed to create user profile for UserId: {UserId}", currentUserId);
                    return ResponseDto<bool>.Failure("Failed to create profile.");
                }

                _logger.LogInformation("User profile created successfully for UserId: {UserId}", currentUserId);

                return ResponseDto<bool>.SuccessResponse(true, "Profile created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user profile.");
                return ResponseDto<bool>.Failure("An error occurred while creating profile.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateAsync(UpdateUserProfileRequest request)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    _logger.LogWarning("Unauthorized attempt to update user profile.");
                    return ResponseDto<bool>.Failure("User not authenticated.");
                }

                var existingProfile = await _userProfileRepository.GetByUserIdAsync(currentUserId);

                if (existingProfile == null)
                {
                    _logger.LogInformation("Attempt to update non-existent profile for UserId: {UserId}", currentUserId);
                    return ResponseDto<bool>.Failure("Profile not found.");
                }

                existingProfile.Gender = request.Gender;
                existingProfile.HeightCm = request.HeightCm;
                existingProfile.CurrentWeightKg = request.CurrentWeightKg;
                existingProfile.GoalWeightKg = request.GoalWeightKg;
                existingProfile.ActivityLevel = request.ActivityLevel;
                existingProfile.FitnessGoal = request.FitnessGoal;
                existingProfile.DateOfBirth = request.DateOfBirth;
                existingProfile.UpdatedAt = DateTime.UtcNow;

                var result = await _userProfileRepository.UpdateAsync(existingProfile);

                if (result <= 0)
                {
                    _logger.LogWarning("Failed to update user profile for UserId: {UserId}", currentUserId);
                    return ResponseDto<bool>.Failure("Failed to update profile.");
                }

                _logger.LogInformation("User profile updated successfully for UserId: {UserId}", currentUserId);

                return ResponseDto<bool>.SuccessResponse(true, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user profile.");
                return ResponseDto<bool>.Failure("An error occurred while updating profile.");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    _logger.LogWarning("Unauthorized attempt to delete user profile.");
                    return ResponseDto<bool>.Failure("User not authenticated.");
                }

                var result = await _userProfileRepository.DeleteAsync(id);

                if (result <= 0)
                {
                    _logger.LogInformation("Attempt to delete non-existent profile with Id: {ProfileId}", id);
                    return ResponseDto<bool>.Failure("Profile not found.");
                }

                _logger.LogInformation("User profile deleted successfully. ProfileId: {ProfileId}, DeletedBy: {UserId}", id, currentUserId);

                return ResponseDto<bool>.SuccessResponse(true, "Profile deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user profile. ProfileId: {ProfileId}", id);
                return ResponseDto<bool>.Failure("An error occurred while deleting profile.");
            }
        }
    }
}