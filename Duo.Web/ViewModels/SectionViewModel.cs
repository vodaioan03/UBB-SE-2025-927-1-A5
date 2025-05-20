using DuoClassLibrary.Models.Sections;

namespace Duo.Web.ViewModels
{
    public class SectionViewModel
    {
        public Section Section { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsUnlocked { get; set; }
        public int CompletedQuizzes { get; set; }
        public List<QuizButtonViewModel> QuizButtons { get; set; }
        public QuizButtonViewModel ExamButton { get; set; }
    }
}
