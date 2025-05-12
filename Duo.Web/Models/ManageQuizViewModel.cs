using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models.Quizzes;

namespace Duo.Web.Models
{
    public class ManageQuizViewModel
    {
        public Quiz Quiz { get; set; }
        public List<Exercise> AssignedExercises { get; set; }
        public List<Exercise> AllExercises { get; set; }
    }

}
