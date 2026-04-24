using GymProject.Models;

namespace GymProject.Repositories.Interfaces
{
    public interface IPlanExercisesRepository
    {
        Task<bool> AddPlanExerciseAsync(PlanExercises planExercise);
        Task<bool> UpdatePlanExerciseAsync(Guid id,PlanExercises planExercise);
        Task<bool> DeletePlanExerciseAsync(Guid id);
        Task<List<PlanExercises>> GetAllPlanExercisesAsync();
        Task<PlanExercises?> GetPlanExerciseByIdAsync(Guid id);
    }
}
