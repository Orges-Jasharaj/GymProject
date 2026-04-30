using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;

namespace GymProject.Services.Interface
{
    public interface IExercisesService
    {
        Task<ResponseDto<ExercisesDto>> CreateExercise(CreateExercisesDto exercise);
        Task<ResponseDto<ExercisesDto>> GetExerciseById(Guid id);
        Task<ResponseDto<List<ExercisesDto>>> GetAllExercises();
        Task<ResponseDto<List<ExercisesDto>>> GetExercisesByMuscleGroup(string muscleGroup);
        Task<ResponseDto<bool>> UpdateExercise(Guid id, CreateExercisesDto exercise);
        Task<ResponseDto<bool>> DeleteExercise(Guid id);
    }
}
