using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Roadmap;
using System.Collections.Generic;
using DuoWebApp.ViewModels;

namespace DuoWebApp.Controllers
{
    public class RoadmapController : Controller
    {
        private readonly IRoadmapService _roadmapService;
        private readonly ISectionService _sectionService;
        private readonly IUserService _userService;

        public RoadmapController(IRoadmapService roadmapService, ISectionService sectionService, IUserService userService)
        {
            _roadmapService = roadmapService;
            _sectionService = sectionService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var roadmap = await _roadmapService.GetByIdAsync(1);
            var user = await _userService.GetByIdAsync(1);
            var sections = await _sectionService.GetByRoadmapId(roadmap.Id);

            int completedSections = user.NumberOfCompletedSections;
            int completedQuizzes = user.NumberOfCompletedQuizzesInSection;

            var sectionViewModels = new List<SectionUnlockViewModel>();

            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                bool isSectionUnlocked = i <= completedSections;

                var quizzes = section.GetAllQuizzes().ToList();
                List<QuizUnlockViewModel> quizViewModels;
                bool isExamUnlocked;

                if (i < completedSections)
                {
                    quizViewModels = quizzes
                        .Select(q => new QuizUnlockViewModel { Quiz = q, IsUnlocked = true })
                        .ToList();
                    isExamUnlocked = false;
                }
                else if (i == completedSections)
                {
                    quizViewModels = quizzes
                        .Select((q, idx) => new QuizUnlockViewModel { Quiz = q, IsUnlocked = idx <= completedQuizzes })
                        .ToList();
                    isExamUnlocked = completedQuizzes >= quizzes.Count;
                }
                else
                {
                    quizViewModels = quizzes
                        .Select(q => new QuizUnlockViewModel { Quiz = q, IsUnlocked = false })
                        .ToList();
                    isExamUnlocked = false;
                }

                bool isExamCompleted = i < completedSections;

                sectionViewModels.Add(new SectionUnlockViewModel
                {
                    Section = section,
                    IsUnlocked = isSectionUnlocked,
                    Quizzes = quizViewModels,
                    Exam = section.Exam,
                    IsExamUnlocked = isExamUnlocked,
                    IsExamCompleted = isExamCompleted
                });
            }

            return View(sectionViewModels);
        }
    }
}