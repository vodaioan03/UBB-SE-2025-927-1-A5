using DuoClassLibrary.Models.Exercises;

namespace Duo.Web.Models
{
    public class CreateQuizViewModel
    {
        public List<Exercise> AvailableExercises { get; set; } = new();
        public List<int> SelectedExerciseIds { get; set; } = new();
    }
}
