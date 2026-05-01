using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.SubscriptionService.Services.Interface
{
    public interface IGymService
    {
        Task<ResponseDto<bool>> CreateGymAsync(CreateGymDto createGymDto);
        Task<ResponseDto<bool>> UpdateGymAsync(Guid gymId, CreateGymDto updateGymDto);
        Task<ResponseDto<bool>> DeleteGymAsync(Guid gymId);
        Task<ResponseDto<List<GymDto>>> GetAllGyms();
        Task<ResponseDto<GymDto>> GetGymByIdAsync(Guid gymId);
    }
}
