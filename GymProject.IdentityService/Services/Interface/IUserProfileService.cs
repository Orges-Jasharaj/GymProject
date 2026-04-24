using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;

namespace GymProject.Services.Interface
{
    public interface IUserProfileService
    {
        Task<ResponseDto<UserProfileDto>> GetMyProfileAsync();
        Task<ResponseDto<bool>> CreateAsync(CreateUserProfileRequest request);
        Task<ResponseDto<bool>> UpdateAsync(UpdateUserProfileRequest request);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
    }
}