using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace GymProject.Services.Implementation
{
    public class UserService : IUser
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly CurrentUserService _currentUserService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IUserEmailStore<User> emailStore,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IUserRepository userRepository,
            CurrentUserService currentUserService,
            ILogger<UserService> logger
            )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = emailStore;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (userExists != null)
                {
                    _logger.LogInformation("Attempt to create user with existing email: {Email}", createUserDto.Email);
                    return ResponseDto<bool>.Failure("User already exists");
                }

                var user = new User
                {
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    DateOfBirth = createUserDto.DateOfBirth,
                    isActive = true,
                };

                await _userStore.SetUserNameAsync(user, createUserDto.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, createUserDto.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, createUserDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleTypes.User);

                    

                    return ResponseDto<bool>.SuccessResponse(true, "User created successfully");
                }

                var errors = result.Errors.Select(e => new ApiError
                {
                    ErrorCode = e.Code,
                    ErrorMessage = e.Description
                }).ToList();

                _logger.LogInformation("User creation failed for email: {Email}. Errors: {Errors}", createUserDto.Email, string.Join(", ", errors.Select(e => e.ErrorMessage)));
                return ResponseDto<bool>.Failure("User creation failed", errors);
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure("An error occurred while creating user");
            }
        }

        public async Task<ResponseDto<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("Attempt to delete non-existent user with ID: {UserId}", userId);
                return ResponseDto<bool>.Failure("User not found.");
            }

            user.isActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var actorId = _currentUserService.GetCurrentUserId();
                var actorEmail = string.IsNullOrWhiteSpace(actorId)
                    ? null
                    : (await _userManager.FindByIdAsync(actorId))?.Email;

                

                return ResponseDto<bool>.SuccessResponse(true, "User deactivated successfully.");
            }

            var errors = result.Errors.Select(e => new ApiError
            {
                ErrorCode = e.Code,
                ErrorMessage = e.Description
            }).ToList();

            _logger.LogInformation("User deactivation failed for user ID: {UserId}. Errors: {Errors}", userId, string.Join(", ", errors.Select(e => e.ErrorMessage)));
            return ResponseDto<bool>.Failure("User deactivation failed.", errors);
        }

        public async Task<ResponseDto<bool>> ReactivateUserAsync(string userId)
        {
            var updated = await _userRepository.ReactivateUserAsync(userId);
            if (!updated)
            {
                _logger.LogInformation(userId, "Attempt to reactivate non-existent user with ID: {UserId}", userId);
                return ResponseDto<bool>.Failure("User not found.");
            }

            var actorId = _currentUserService.GetCurrentUserId();
            var actorEmail = string.IsNullOrWhiteSpace(actorId)
                ? null
                : (await _userManager.FindByIdAsync(actorId))?.Email;

            

            return ResponseDto<bool>.SuccessResponse(true, "User reactivated successfully.");
        }

        public async Task<ResponseDto<List<UserDto>>> GetAllUsersAsync(ClaimsPrincipal currentUser)
        {
            try
            {
                var includeInactive = currentUser.IsInRole(RoleTypes.Admin) || currentUser.IsInRole(RoleTypes.SuperAdmin);
                var users = await _userRepository.GetAllUsersAsync(includeInactive);
                return ResponseDto<List<UserDto>>.SuccessResponse(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
                return ResponseDto<List<UserDto>>.Failure("An error occurred while retrieving users");
            }
        }

        public async Task<ResponseDto<UserDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("Attempt to retrieve non-existent user with ID: {UserId}", userId);
                return ResponseDto<UserDto>.Failure("User not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
            };

            return ResponseDto<UserDto>.SuccessResponse(userDto, "User retrieved successfully.");
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var userExists = await _userManager.FindByEmailAsync(loginDto.Email);
            if (userExists == null)
            {
                _logger.LogInformation("Login attempt with non-existent email: {Email}", loginDto.Email);
                return ResponseDto<LoginResponseDto>.Failure("User does not exist");
            }

            var result = await _signInManager.PasswordSignInAsync(userExists, loginDto.Password, false, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(userExists);

                var token = _tokenService.GenerateAccessToken(userExists, roles.ToList());
                var refreshToken = _tokenService.GenerateRrefreshToken();
                userExists.RefreshToken = refreshToken.RefreshToken;
                userExists.RefreshTokenExpiryTime = refreshToken.RefreshTokenExpiryDate;

                await _userManager.UpdateAsync(userExists);

                var rolesList = roles.ToList();
                var loginResponse = new LoginResponseDto
                {
                    DisplayName = $"{userExists.FirstName} {userExists.LastName}",
                    Email = userExists.Email,
                    AccessToken = token,
                    RefreshToken = refreshToken.RefreshToken,
                    RefreshTokenExpiryTime = refreshToken.RefreshTokenExpiryDate,
                    Roles = rolesList
                };

                return ResponseDto<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful");
            }

            _logger.LogInformation("Login failed for email: {Email}", loginDto.Email);
            return ResponseDto<LoginResponseDto>.Failure("Login failed, please check your credentials");
        }

        public async Task<ResponseDto<bool>> UpdateUserAsync(string userId, UpdateUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogInformation("Attempt to update non-existent user with ID: {UserId}", userId);
                return ResponseDto<bool>.Failure("User not found.");
            }

            var oldValues = new
            {
                user.FirstName,
                user.LastName,
                user.DateOfBirth
            };

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.DateOfBirth = userDto.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var actorId = _currentUserService.GetCurrentUserId();
                var actorEmail = string.IsNullOrWhiteSpace(actorId)
                    ? null
                    : (await _userManager.FindByIdAsync(actorId))?.Email;

                

                return ResponseDto<bool>.SuccessResponse(true, "User updated successfully.");
            }

            var errors = result.Errors.Select(e => new ApiError
            {
                ErrorCode = e.Code,
                ErrorMessage = e.Description
            }).ToList();

            _logger.LogInformation("User update failed for user ID: {UserId}. Errors: {Errors}", userId, string.Join(", ", errors.Select(e => e.ErrorMessage)));
            return ResponseDto<bool>.Failure("User update failed.", errors);
        }

        public async Task<ResponseDto<bool>> ChangeUserPassword(ChangePasswordDto changePasswordDto)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return ResponseDto<bool>.Failure("User not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                return ResponseDto<bool>.Failure("User does not exist");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                
                return ResponseDto<bool>.SuccessResponse(true, "Password changed successfully");
            }

            var errors = result.Errors.Select(e => new ApiError
            {
                ErrorCode = e.Code,
                ErrorMessage = e.Description
            }).ToList();

            _logger.LogInformation("Password change failed for user ID: {UserId}. Errors: {Errors}", currentUserId, string.Join(", ", errors.Select(e => e.ErrorMessage)));
            return ResponseDto<bool>.Failure("Password change failed", errors);
        }

        public async Task<ResponseDto<LoginResponseDto>> RefreshToken(RefreshTokenRequestDto refreshTokenDto)
        {
            var claimPrincipal = _tokenService.GetClaimsPrincipal(refreshTokenDto.AccessToken);
            if (claimPrincipal == null)
            {
                return ResponseDto<LoginResponseDto>.Failure("Invalid access token");
            }

            var userId = claimPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return ResponseDto<LoginResponseDto>.Failure("User does not exist");
            }

            if (user.RefreshToken != refreshTokenDto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return ResponseDto<LoginResponseDto>.Failure("Invalid or expired refresh token");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateAccessToken(user, roles.ToList());
            var refreshToken = _tokenService.GenerateRrefreshToken();
            user.RefreshToken = refreshToken.RefreshToken;
            user.RefreshTokenExpiryTime = refreshToken.RefreshTokenExpiryDate;

            await _userManager.UpdateAsync(user);

            var rolesList = roles.ToList();
            var loginResponse = new LoginResponseDto
            {
                DisplayName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                AccessToken = token,
                RefreshToken = refreshToken.RefreshToken,
                RefreshTokenExpiryTime = refreshToken.RefreshTokenExpiryDate,
                Roles = rolesList
            };

            return ResponseDto<LoginResponseDto>.SuccessResponse(loginResponse, "Token refreshed successfully");
        }

        public async Task<ResponseDto<bool>> CreateUserWithRoleAsync(CreateUserDto createUserDto, string role)
        {
            try
            {
                var allowedRoles = new[] { RoleTypes.SuperAdmin, RoleTypes.Admin };

                if (!allowedRoles.Contains(role))
                {
                    _logger.LogInformation("Attempt to create user with invalid role: {Role}. Allowed roles are: {AllowedRoles}", role, string.Join(", ", allowedRoles));
                    return ResponseDto<bool>.Failure($"Invalid role: {role}. Allowed roles are: {string.Join(", ", allowedRoles)}");
                }

                var userExists = await _userManager.FindByEmailAsync(createUserDto.Email);
                if (userExists != null)
                {
                    _logger.LogInformation("Attempt to create user with existing email: {Email}", createUserDto.Email);
                    return ResponseDto<bool>.Failure("User already exists");
                }

                var user = new User
                {
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    DateOfBirth = createUserDto.DateOfBirth,
                    isActive = true
                };

                await _userStore.SetUserNameAsync(user, createUserDto.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, createUserDto.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, createUserDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);

                    var actorId = _currentUserService.GetCurrentUserId();
                    

                    return ResponseDto<bool>.SuccessResponse(true, $"User created successfully as {role}");
                }

                var errors = result.Errors.Select(e => new ApiError
                {
                    ErrorCode = e.Code,
                    ErrorMessage = e.Description
                }).ToList();

                _logger.LogInformation("User creation with role {Role} failed for email: {Email}. Errors: {Errors}", role, createUserDto.Email, string.Join(", ", errors.Select(e => e.ErrorMessage)));
                return ResponseDto<bool>.Failure("User creation failed", errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user with role {Role} for email: {Email}", role, createUserDto.Email);
                return ResponseDto<bool>.Failure("An error occurred while creating user");
            }
        }
    }
}
