using Duo.Web.Models;

namespace Duo.Web.ViewModels
{
    public class QuizPreviewViewModel
    {
        public DuoClassLibrary.Models.Quizzes.Quiz Quiz { get; set; }

        public List<int> ExerciseIds { get; set; }
        public string SectionTitle { get; set; }
    }
}
