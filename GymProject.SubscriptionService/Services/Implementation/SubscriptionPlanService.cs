using Dapper;
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
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ILogger<SubscriptionPlanService> _logger;
        private readonly EntityContext _context;
        private readonly DapperContext _dapperContext;
        private readonly IAuditLogService _auditLogService;
        private readonly CurrentUserService _currentUserService;

        public SubscriptionPlanService(ILogger<SubscriptionPlanService> logger, EntityContext context, DapperContext dapperContext, IAuditLogService auditLogService, CurrentUserService currentUserService)
        {
            _logger = logger;
            _context = context;
            _dapperContext = dapperContext;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }


        public async Task<ResponseDto<bool>> CreateSubscriptionPlan(CreateSubscriptionPlanDto createSubscriptionPlanDto)
        {
            var existingPlan = await _context.Subscriptions.FirstOrDefaultAsync(sp => sp.GymId == createSubscriptionPlanDto.GymId && sp.Name == createSubscriptionPlanDto.Name);

            if (existingPlan != null)
            {
                return ResponseDto<bool>.Failure("A subscription plan with the same name already exists for this gym.");
            }

            var subscriptionPlan = new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                GymId = createSubscriptionPlanDto.GymId,
                Name = createSubscriptionPlanDto.Name,
                Price = createSubscriptionPlanDto.Price,
                DurationInDays = createSubscriptionPlanDto.DurationInDays,
                CreatedBy = _currentUserService.GetCurrentUserId(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscriptionPlan);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActivityAsync<SubscriptionPlan>(
                    _currentUserService.GetCurrentUserId(),
                    "Create",
                    "SubscriptionPlan",
                    subscriptionPlan.Id.ToString(),
                    null,
                    subscriptionPlan);
            return ResponseDto<bool>.SuccessResponse(true, "Subscription plan created successfully.");
        }

        public async Task<ResponseDto<bool>> DeleteSubscriptionPlan(Guid id)
        {
            var subscriptionPlan = await _context.Subscriptions.FindAsync(id);
            if (subscriptionPlan == null)
            {
                return ResponseDto<bool>.Failure("Subscription plan not found.");
            }

            _context.Subscriptions.Remove(subscriptionPlan);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActivityAsync<SubscriptionPlan>(
                    _currentUserService.GetCurrentUserId(),
                    "Delete",
                    "SubscriptionPlan",
                    subscriptionPlan.Id.ToString(),
                    subscriptionPlan,
                    null);
            return ResponseDto<bool>.SuccessResponse(true, "Subscription plan deleted successfully.");
        }

        public async Task<ResponseDto<List<SubscriptionPlanDto>>> GetAllSubscriptionPlans()
        {
            try
            {
                var query = "SELECT * FROM Subscriptions";
                using var connection = _dapperContext.CreateConnection();
                var subscriptionPlans = await connection.QueryAsync<SubscriptionPlanDto>(query);
                return ResponseDto<List<SubscriptionPlanDto>>.SuccessResponse(subscriptionPlans.ToList(), "Subscription plans fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription plans");
                return ResponseDto<List<SubscriptionPlanDto>>.Failure("An error occurred while fetching subscription plans.");
            }
        }

        public async Task<ResponseDto<SubscriptionPlanDto>> GetSubscriptionPlanById(Guid id)
        {
            try
            {
                var query = "SELECT * FROM Subscriptions WHERE Id = @Id";
                using var connection = _dapperContext.CreateConnection();
                var subscriptionPlan = await connection.QueryFirstOrDefaultAsync<SubscriptionPlanDto>(query, new { Id = id });
                if (subscriptionPlan == null)
                {
                    return ResponseDto<SubscriptionPlanDto>.Failure("Subscription plan not found.");
                }
                return ResponseDto<SubscriptionPlanDto>.SuccessResponse(subscriptionPlan, "Subscription plan fetched successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error fetching subscription plan with ID {id}");
                return ResponseDto<SubscriptionPlanDto>.Failure("An error occurred while fetching the subscription plan.");
            }
        }

        public async Task<ResponseDto<List<SubscriptionPlanDto>>> GetSubscriptionPlansByGymId(Guid gymId)
        {
            try
            {
                var query = "SELECT * FROM Subscriptions WHERE GymId = @GymId";
                using var connection = _dapperContext.CreateConnection();
                var subscriptionPlans = await connection.QueryAsync<SubscriptionPlanDto>(query, new { GymId = gymId });
                return ResponseDto<List<SubscriptionPlanDto>>.SuccessResponse(subscriptionPlans.ToList(), "Subscription plans for the gym fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching subscription plans for gym ID {gymId}");
                return ResponseDto<List<SubscriptionPlanDto>>.Failure("An error occurred while fetching subscription plans for the gym.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateSubscriptionPlan(Guid id, CreateSubscriptionPlanDto updateSubscriptionPlanDto)
        {
            var subscriptionPlan = await _context.Subscriptions.FindAsync(id);
            if (subscriptionPlan == null)
            {
                return ResponseDto<bool>.Failure("Subscription plan not found.");
            }

            var currectuser = _currentUserService.GetCurrentUserId();

            var oldSubscriptionPlan = new SubscriptionPlan
            {
                Id = subscriptionPlan.Id,
                GymId = subscriptionPlan.GymId,
                Name = subscriptionPlan.Name,
                Price = subscriptionPlan.Price,
                DurationInDays = subscriptionPlan.DurationInDays,
                CreatedBy = subscriptionPlan.CreatedBy,
                CreatedAt = subscriptionPlan.CreatedAt
            };

            subscriptionPlan.Name = updateSubscriptionPlanDto.Name;
            subscriptionPlan.Price = updateSubscriptionPlanDto.Price;
            subscriptionPlan.DurationInDays = updateSubscriptionPlanDto.DurationInDays;
            subscriptionPlan.UpdatedBy = currectuser;
            subscriptionPlan.UpdatedAt = DateTime.UtcNow;

            _context.Subscriptions.Update(subscriptionPlan);

            await _context.SaveChangesAsync();

            await _auditLogService.LogActivityAsync<SubscriptionPlan>(
                    currectuser,
                    "Update",
                    "SubscriptionPlan",
                    subscriptionPlan.Id.ToString(),
                    oldSubscriptionPlan,
                    subscriptionPlan);
            return ResponseDto<bool>.SuccessResponse(true, "Subscription plan updated successfully.");
        }
    }
}
