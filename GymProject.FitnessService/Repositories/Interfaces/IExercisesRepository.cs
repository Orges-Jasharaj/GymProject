using GymProject.Models;

namespace GymProject.Repositories.Interfaces
{
    public interface IExercisesRepository
    {
        Task<bool> CreateExercise(Exercises exercise);
        Task<Exercises> GetExerciseById(Guid id);
        Task<List<Exercises>> GetAllExercises();
        Task<List<Exercises>> GetExercisesByMuscleGroup(string muscleGroup);
        Task<bool> UpdateExercise(Guid id,Exercises exercise);
        Task<bool> DeleteExercise(Guid id);
    }
}
