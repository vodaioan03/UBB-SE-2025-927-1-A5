using DuoClassLibrary.Models.Quizzes;

namespace Duo.Web.ViewModels
{
    public class QuizButtonViewModel
    {
        public BaseQuiz Quiz { get; set; }
        public QuizButtonStatus Status { get; set; }

        // Helper properties for the view
        public bool IsLocked => Status == QuizButtonStatus.Locked;
        public bool IsCompleted => Status == QuizButtonStatus.Completed;
        public bool IsIncomplete => Status == QuizButtonStatus.Incomplete;
    }

    public enum QuizButtonStatus
    {
        Locked = 1,
        Completed = 2,
        Incomplete = 3
    }
}
