using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Models.Sections;
using DuoClassLibrary.Services;
using Duo.Web.ViewModels;

namespace DuoWebApp.Controllers
{
    public class RoadmapController(
        IRoadmapService roadmapService,
        ISectionService sectionService,
        IUserService userService,
        IQuizService quizService) : Controller
    {
        private readonly IRoadmapService _roadmapService = roadmapService ?? throw new ArgumentNullException(nameof(roadmapService));
        private readonly ISectionService _sectionService = sectionService ?? throw new ArgumentNullException(nameof(sectionService));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly IQuizService _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));

        public async Task<IActionResult> Index()
        {
            try
            {
                var roadmap = await _roadmapService.GetByIdAsync(1);
                var user = await _userService.GetByIdAsync(1);
                List<Section> sections = await _sectionService.GetByRoadmapId(1);

                if (sections == null || sections.Count == 0)
                {
                    return View("Error", "This roadmap does not contain any sections yet.");
                }

                var roadmapViewModel = new RoadmapViewModel
                {
                    Roadmap = roadmap,
                    Sections = new List<SectionViewModel>()
                };

                bool isPreviousSectionCompleted = true;
                bool currentSectionIsCompleted = false;

                for (int i = 0; i < sections.Count; i++)
                {
                    Section section = sections[i];
                    currentSectionIsCompleted = await _sectionService.IsSectionCompleted(user.UserId, section.Id);

                    var sectionViewModel = new SectionViewModel
                    {
                        Section = section,
                        IsCompleted = currentSectionIsCompleted,
                        IsUnlocked = isPreviousSectionCompleted,
                        QuizButtons = new List<QuizButtonViewModel>()
                    };

                    // Get quizzes for this section
                    List<Quiz> quizzes = await _quizService.GetAllQuizzesFromSection(section.Id);
                    section.Quizzes = quizzes;

                    // Get exam for this section
                    Exam exam = await _quizService.GetExamFromSection(section.Id);
                    section.Exam = exam;

                    // Get completed quizzes status
                    List<bool> completedQuizzes = new List<bool>();
                    foreach (Quiz quiz in quizzes)
                    {
                        completedQuizzes.Add(await _quizService.IsQuizCompleted(user.UserId, quiz.Id));
                    }

                    // Populate quiz buttons with status
                    bool previousQuizIsCompleted = isPreviousSectionCompleted;
                    bool currentQuizIsCompleted = false;

                    for (int q = 0; q < quizzes.Count; q++)
                    {
                        Quiz quiz = quizzes[q];
                        currentQuizIsCompleted = completedQuizzes[q];

                        QuizButtonStatus quizStatus = QuizButtonStatus.Locked;

                        if (sectionViewModel.IsCompleted)
                        {
                            quizStatus = QuizButtonStatus.Completed;
                        }
                        else if (currentQuizIsCompleted)
                        {
                            quizStatus = QuizButtonStatus.Completed;
                        }
                        else if (previousQuizIsCompleted && !currentQuizIsCompleted)
                        {
                            quizStatus = QuizButtonStatus.Incomplete;
                        }

                        sectionViewModel.QuizButtons.Add(new QuizButtonViewModel
                        {
                            Quiz = quiz,
                            Status = quizStatus
                        });

                        previousQuizIsCompleted = currentQuizIsCompleted;
                    }

                    // Set up the exam button
                    QuizButtonStatus examStatus = QuizButtonStatus.Locked;
                    if (sectionViewModel.IsCompleted)
                    {
                        examStatus = QuizButtonStatus.Completed;
                    }
                    else if (previousQuizIsCompleted)
                    {
                        examStatus = QuizButtonStatus.Incomplete;
                    }

                    sectionViewModel.ExamButton = new QuizButtonViewModel
                    {
                        Quiz = exam,
                        Status = examStatus
                    };

                    roadmapViewModel.Sections.Add(sectionViewModel);
                    isPreviousSectionCompleted = currentSectionIsCompleted;
                }

                return View(roadmapViewModel);
            }
            catch (Exception ex)
            {
                return View("Error", $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> QuizPreview(int id, bool isUnlocked)
        {
            try
            {
                if (!isUnlocked)
                {
                    return View("Error", "This quiz is locked. Complete previous quizzes first.");
                }

                var quiz = await _quizService.GetQuizById(id);
                if (quiz == null)
                {
                    // Try to get as exam if not found as quiz
                    var exam = await _quizService.GetExamById(id);
                    if (exam == null)
                    {
                        return View("Error", $"Quiz or exam with ID {id} not found.");
                    }

                    return View("ExamPreview", exam);
                }

                return View(quiz);
            }
            catch (Exception ex)
            {
                return View("Error", $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> StartQuiz(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizById(id);
                if (quiz == null)
                {
                    return View("Error", $"Quiz with ID {id} not found.");
                }

                return RedirectToAction("Quiz", "QuizExam", new { id });
            }
            catch (Exception ex)
            {
                return View("Error", $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> StartExam(int id)
        {
            try
            {
                var exam = await _quizService.GetExamById(id);
                if (exam == null)
                {
                    return View("Error", $"Exam with ID {id} not found.");
                }

                return RedirectToAction("Exam", "QuizExam", new { id });
            }
            catch (Exception ex)
            {
                return View("Error", $"An error occurred: {ex.Message}");
            }
        }
    }
}