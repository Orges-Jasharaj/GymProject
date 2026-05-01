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
    public class GymService : IGymService
    {
        private readonly EntityContext _context;
        private readonly DapperContext _dapperContext;
        private readonly ILogger<GymService> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly CurrentUserService _currentUserService;

        public GymService(EntityContext context, DapperContext dapperContext, ILogger<GymService> logger, IAuditLogService auditLogService, CurrentUserService currentUserService)
        {
            _context = context;
            _dapperContext = dapperContext;
            _logger = logger;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }


        public async Task<ResponseDto<bool>> CreateGymAsync(CreateGymDto createGymDto)
        {
            var exitinggym = await _context.Gyms.FirstOrDefaultAsync(g => g.Name == createGymDto.Name && g.City == createGymDto.City);

            if (exitinggym != null)
            {
                return ResponseDto<bool>.Failure("Gym with the same name and city already exists.");
            }

            var gym = new Gym
            {
                Id = Guid.NewGuid(),
                Name = createGymDto.Name,
                City = createGymDto.City,
                CreatedBy = _currentUserService.GetCurrentUserId(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Gyms.Add(gym);

            var result = await _context.SaveChangesAsync();

            if(result > 0)
            {
                await _auditLogService.LogActivityAsync<Gym>(
                    _currentUserService.GetCurrentUserId(),
                    "Create",
                    "Meals",
                    gym.Id.ToString(),
                    null,
                    gym);
                return ResponseDto<bool>.SuccessResponse(true, "Gym created successfully.");
            }
            else
            {
                return ResponseDto<bool>.Failure("Failed to create gym.");
            }

        }

        public async Task<ResponseDto<bool>> DeleteGymAsync(Guid gymId)
        {
            var gym = await _context.Gyms.FindAsync(gymId);
            if (gym == null)
            {
                return ResponseDto<bool>.Failure("Gym not found.");
            }

            _context.Gyms.Remove(gym);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _auditLogService.LogActivityAsync<Gym>(
                    _currentUserService.GetCurrentUserId(),
                    "Delete",
                    "Gyms",
                    gym.Id.ToString(),
                    null,
                    gym);
                return ResponseDto<bool>.SuccessResponse(true, "Gym deleted successfully.");
            }
            else
            {
                return ResponseDto<bool>.Failure("Failed to delete gym.");
            }
        }

        public async Task<ResponseDto<List<GymDto>>> GetAllGyms()
        {
            //use dapper here
            try
            {
                var query = "SELECT * FROM Gyms";
                using var connection = _dapperContext.CreateConnection();
                var gyms = await connection.QueryAsync<GymDto>(query);
                return ResponseDto<List<GymDto>>.SuccessResponse(gyms.ToList(), "Gyms fetched successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching gyms");
                return ResponseDto<List<GymDto>>.Failure("An error occurred while fetching gyms.");
            }
        }

        public async Task<ResponseDto<GymDto>> GetGymByIdAsync(Guid gymId)
        {
            try
            {
                var query = "SELECT * FROM Gyms WHERE Id = @Id";
                using var connection = _dapperContext.CreateConnection();
                var gym = await connection.QueryFirstOrDefaultAsync<GymDto>(query, new { Id = gymId });
                if (gym == null)
                {
                    return ResponseDto<GymDto>.Failure("Gym not found.");
                }
                return ResponseDto<GymDto>.SuccessResponse(gym, "Gym fetched successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error fetching gym with id {gymId}");
                return ResponseDto<GymDto>.Failure("An error occurred while fetching the gym.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateGymAsync(Guid gymId, CreateGymDto updateGymDto)
        {
            var gym = await _context.Gyms.FindAsync(gymId);
            if (gym == null)
            {
                return ResponseDto<bool>.Failure("Gym not found.");
            }

            var currectuserId = _currentUserService.GetCurrentUserId();

            var oldvalues = new Gym
            {
                Id = gym.Id,
                Name = gym.Name,
                City = gym.City,
                CreatedBy = gym.CreatedBy,
                CreatedAt = gym.CreatedAt,
                UpdatedBy = gym.UpdatedBy,
                UpdatedAt = gym.UpdatedAt
            };

            gym.Name = updateGymDto.Name;
            gym.City = updateGymDto.City;
            gym.UpdatedBy = currectuserId;
            gym.UpdatedAt = DateTime.UtcNow;

            if (await _context.SaveChangesAsync() > 0)
            {
                await _auditLogService.LogActivityAsync<Gym>(
                    currectuserId,
                    "Update",
                    "Gyms",
                    gym.Id.ToString(),
                    oldvalues,
                    gym);
                return ResponseDto<bool>.SuccessResponse(true, "Gym updated successfully.");
            }
            else
            {
                return ResponseDto<bool>.Failure("Failed to update gym.");
            }
        }
    }
}
