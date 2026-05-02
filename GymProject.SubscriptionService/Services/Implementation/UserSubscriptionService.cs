using GymProject.Dtos.Responses;
using GymProject.Services.Implementation;
using GymProject.Services.Interface;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;
using GymProject.SubscriptionService.Data;
using GymProject.SubscriptionService.Models;
using GymProject.SubscriptionService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace GymProject.SubscriptionService.Services.Implementation
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly EntityContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly CurrentUserService _currentUserService;

        public UserSubscriptionService(EntityContext context, IAuditLogService auditLogService, CurrentUserService currentUserService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseDto<bool>> CreateUserSubscription(CreateUserSubscriptionDto createUserSubscriptionDto)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            var userId = string.IsNullOrWhiteSpace(createUserSubscriptionDto.UserId)
                ? currentUserId
                : createUserSubscriptionDto.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<bool>.Failure("User id is required to create a subscription.");
            }

            var actorId = currentUserId ?? userId;

            var subscriptionPlan = await _context.Subscriptions.FindAsync(createUserSubscriptionDto.SubscriptionPlanId);
            if (subscriptionPlan == null)
            {
                return ResponseDto<bool>.Failure("Subscription plan not found.");
            }

            var startDate = createUserSubscriptionDto.StartDate?.ToUniversalTime() ?? DateTime.UtcNow;
            var endDate = startDate.AddDays(subscriptionPlan.DurationInDays);

            var activeSubscriptionsForGym = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive)
                .Join(_context.Subscriptions,
                    us => us.SubscriptionPlanId,
                    sp => sp.Id,
                    (us, sp) => new { UserSubscription = us, GymId = sp.GymId })
                .Where(join => join.GymId == subscriptionPlan.GymId)
                .Select(join => join.UserSubscription)
                .OrderByDescending(us => us.EndDate)
                .ToListAsync();

            var validSubscription = activeSubscriptionsForGym.FirstOrDefault();
            if (validSubscription != null && validSubscription.EndDate > DateTime.UtcNow)
            {
                validSubscription.EndDate = validSubscription.EndDate.AddDays(subscriptionPlan.DurationInDays);
                validSubscription.UpdatedBy = actorId;
                validSubscription.UpdatedAt = DateTime.UtcNow;

                foreach (var duplicateSubscription in activeSubscriptionsForGym.Skip(1))
                {
                    duplicateSubscription.IsActive = false;
                    duplicateSubscription.UpdatedBy = actorId;
                    duplicateSubscription.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                await _auditLogService.LogActivityAsync<UserSubscription>(
                    actorId,
                    "Extend",
                    "UserSubscription",
                    validSubscription.Id.ToString(),
                    null,
                    validSubscription);

                return ResponseDto<bool>.SuccessResponse(true, "Existing subscription extended successfully.");
            }

            foreach (var existingSubscription in activeSubscriptionsForGym)
            {
                existingSubscription.IsActive = false;
                existingSubscription.UpdatedBy = actorId;
                existingSubscription.UpdatedAt = DateTime.UtcNow;
            }

            var userSubscription = new UserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionPlanId = subscriptionPlan.Id,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                CreatedBy = actorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSubscriptions.Add(userSubscription);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActivityAsync<UserSubscription>(
                currentUserId,
                "Create",
                "UserSubscription",
                userSubscription.Id.ToString(),
                null,
                userSubscription);

            return ResponseDto<bool>.SuccessResponse(true, "User subscription created successfully.");
        }

        public async Task<ResponseDto<List<UserSubscriptionDto>>> GetUserSubscriptionsByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<List<UserSubscriptionDto>>.Failure("User id is required.");
            }

            var subscriptions = await _context.UserSubscriptions
                .Where(us => us.UserId == userId)
                .Join(_context.Subscriptions,
                    us => us.SubscriptionPlanId,
                    sp => sp.Id,
                    (us, sp) => new UserSubscriptionDto
                    {
                        Id = us.Id,
                        UserId = us.UserId,
                        SubscriptionPlanId = us.SubscriptionPlanId,
                        SubscriptionPlanName = sp.Name,
                        GymId = sp.GymId,
                        StartDate = us.StartDate,
                        EndDate = us.EndDate,
                        IsActive = us.IsActive,
                        CreatedBy = us.CreatedBy,
                        CreatedAt = us.CreatedAt,
                        UpdatedBy = us.UpdatedBy,
                        UpdatedAt = us.UpdatedAt
                    })
                .ToListAsync();

            return ResponseDto<List<UserSubscriptionDto>>.SuccessResponse(subscriptions, "User subscriptions fetched successfully.");
        }

        public async Task<ResponseDto<UserSubscriptionDto>> GetActiveUserSubscription(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<UserSubscriptionDto>.Failure("User id is required.");
            }

            var subscription = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive)
                .Join(_context.Subscriptions,
                    us => us.SubscriptionPlanId,
                    sp => sp.Id,
                    (us, sp) => new UserSubscriptionDto
                    {
                        Id = us.Id,
                        UserId = us.UserId,
                        SubscriptionPlanId = us.SubscriptionPlanId,
                        SubscriptionPlanName = sp.Name,
                        GymId = sp.GymId,
                        StartDate = us.StartDate,
                        EndDate = us.EndDate,
                        IsActive = us.IsActive,
                        CreatedBy = us.CreatedBy,
                        CreatedAt = us.CreatedAt,
                        UpdatedBy = us.UpdatedBy,
                        UpdatedAt = us.UpdatedAt
                    })
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return ResponseDto<UserSubscriptionDto>.Failure("Active subscription not found for this user.");
            }

            return ResponseDto<UserSubscriptionDto>.SuccessResponse(subscription, "Active subscription fetched successfully.");
        }
    }
}
