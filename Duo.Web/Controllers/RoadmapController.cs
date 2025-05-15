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

                var quizViewModels = new List<QuizUnlockViewModel>();
                bool isExamUnlocked = false;

                if (i < completedSections)
                {
                    // Section fully completed, all quizzes and exam unlocked
                    quizViewModels = quizzes.Select(q => new QuizUnlockViewModel { Quiz = q, IsUnlocked = true }).ToList();
                    isExamUnlocked = true;
                }
                else if (i == completedSections)
                {
                    // Current section: unlock quizzes up to completedQuizzes
                    for (int q = 0; q < quizzes.Count; q++)
                    {
                        bool isQuizUnlocked = q <= completedQuizzes;
                        quizViewModels.Add(new QuizUnlockViewModel { Quiz = quizzes[q], IsUnlocked = isQuizUnlocked });
                    }
                    // Exam unlocked only if all quizzes are completed
                    isExamUnlocked = completedQuizzes >= quizzes.Count;
                }
                else
                {
                    // Future sections: nothing unlocked
                    quizViewModels = quizzes.Select(q => new QuizUnlockViewModel { Quiz = q, IsUnlocked = false }).ToList();
                    isExamUnlocked = false;
                }

                sectionViewModels.Add(new SectionUnlockViewModel
                {
                    Section = section,
                    IsUnlocked = isSectionUnlocked,
                    Quizzes = quizViewModels,
                    Exam = section.Exam,
                    IsExamUnlocked = isExamUnlocked
                });
            }

            return View(sectionViewModels);
        }

        public async Task<IActionResult> RecalculateProgress(int userId, int roadmapId, int deletedSectionOrder)
        {
            var user = await _userService.GetByIdAsync(userId);
            var sections = await _sectionService.GetByRoadmapId(roadmapId);

            // Sort sections by OrderNumber (or by Id if OrderNumber is null)
            var orderedSections = sections
                .OrderBy(s => s.OrderNumber ?? int.MaxValue)
                .ToList();

            // Clamp progress if out of bounds
            if (user.NumberOfCompletedSections >= orderedSections.Count)
                user.NumberOfCompletedSections = orderedSections.Count - 1;

            if (user.NumberOfCompletedSections == deletedSectionOrder)
            {
                // Stay at the same index (which now points to the next section)
                // Reset quiz counter to 1
                user.NumberOfCompletedQuizzesInSection = 1;
            }
            else if (deletedSectionOrder < user.NumberOfCompletedSections)
            {
                // Move back one section
                user.NumberOfCompletedSections--;
                // Quiz counter stays the same
            }
            // else: deleted section is after current, do nothing

            await _userService.UpdateUserAsync(user);

            return RedirectToAction("ManageSection", "Section");
        }

    }
}