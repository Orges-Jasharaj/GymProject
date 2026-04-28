using GymProject.Models;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.Repositories.Interfaces
{
    public interface IFitnessPlansRepository
    {
        Task<bool> CreateFitnessPlan(FitnessPlans fitnessPlan);
        Task<FitnessPlans?> GetFitnessPlanById(Guid id);
        Task<List<FitnessPlans>> GetAllFitnessPlans(Guid userId);
        Task<bool> UpdateFitnessPlan(FitnessPlans fitnessPlan);
        Task<bool> DeleteFitnessPlan(Guid id);
        Task<FitnessPlanDetailsDto?> GetFitnessPlanDetails(Guid id);
    }
}
