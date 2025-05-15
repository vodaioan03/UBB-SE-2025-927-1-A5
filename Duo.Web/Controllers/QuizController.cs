// QuizController.cs

using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Models.Exercises;
using System.Linq;
using System.Threading.Tasks;
using Duo.Web.ViewModels;

namespace Duo.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IExerciseService _exerciseService;
        private readonly ISectionService _sectionService;

        public QuizController(IQuizService quizService,
                              IExerciseService exerciseService,
                              ISectionService sectionService)
        {
            _quizService = quizService;
            _exerciseService = exerciseService;
            _sectionService = sectionService;
        }

        // GET /Quiz/ViewQuizzes?selectedQuizId=5
        [HttpGet]
        public async Task<IActionResult> ViewQuizzes(int? selectedQuizId)
        {
            var allQuizzes = await _quizService.GetAllQuizzes();

            var vm = new ManageQuizViewModel
            {
                Quizzes = allQuizzes,
                SelectedQuizId = selectedQuizId
            };

            if (selectedQuizId.HasValue && selectedQuizId > 0)
            {
                var quizExists = allQuizzes.Any(q => q.Id == selectedQuizId.Value);
                if (!quizExists)
                {
                    TempData["Warning"] = $"Quiz with ID {selectedQuizId} no longer exists.";
                    vm.SelectedQuizId = null;
                    vm.AssignedExercises = new List<Exercise>();
                    vm.AvailableExercises = new List<Exercise>();
                    return View(vm);
                }

                try
                {
                    vm.AssignedExercises = await _exerciseService
                        .GetAllExercisesFromQuiz(selectedQuizId.Value);

                    var all = await _exerciseService.GetAllExercises();
                    vm.AvailableExercises = all
                        .Where(e => !vm.AssignedExercises.Any(a => a.ExerciseId == e.ExerciseId))
                        .ToList();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to fetch exercises for quiz ID {selectedQuizId}: {ex.Message}");
                    vm.AssignedExercises = new List<Exercise>();
                    vm.AvailableExercises = new List<Exercise>();
                    TempData["Error"] = "Could not load exercises for this quiz.";
                }
            }

            return View(vm);
        }

        [HttpGet("Quiz/{id}/Preview")]
        public async Task<IActionResult> Preview(int id)
        {
            var quiz = await _quizService.GetQuizById(id);

            if (quiz == null) // Check for null quiz FIRST
            {
                return NotFound(); // Return NotFound if quiz doesn't exist
            }

            string sectionTitle = "Section: ";
            if (quiz.SectionId != null)
            {
                var section = await _sectionService.GetSectionById(quiz.SectionId.Value);
                if (section != null) // It's also good practice to check if the section was found
                {
                    sectionTitle += section.Title;
                }
                // else: sectionTitle will remain "Section: " or you can handle missing section
            }

            var exercises = await _exerciseService.GetAllExercisesFromQuiz(id);
            var viewModel = new QuizPreviewViewModel
            {
                Quiz = quiz,
                ExerciseIds = exercises.Select(e => e.ExerciseId).ToList(),
                SectionTitle = sectionTitle
            };

            return View("QuizPreview", viewModel); // Pass the populated viewModel to the view
        }

        [HttpGet("Quiz/{id}/Solve")]
        public async Task<IActionResult> Solve(int id, int? index)
        {
            var quiz = await _quizService.GetQuizById(id);
            if (quiz == null || quiz.Exercises == null || !quiz.Exercises.Any())
                return NotFound();

            int idx = index.GetValueOrDefault(0);
            if (idx < 0 || idx >= quiz.Exercises.Count)
                return RedirectToAction(nameof(Solve), new { id, index = 0 });

            var vm = new QuizSolverViewModel
            {
                QuizId = quiz.Id,
                QuizTitle = quiz is Exam ? "Final Exam" : $"Quiz {quiz.Id}",
                AllExercises = quiz.Exercises.ToList(),

                CurrentExerciseIndex = idx,
                CurrentExercise = quiz.Exercises[idx],
                CurrentExerciseType = quiz.Exercises[idx].GetType().Name,

                IsLastExercise = (idx == quiz.Exercises.Count - 1)
            };

            return View("Solve", vm);
        }


        // POST /Quiz/AddExercise
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExercise(int quizId, int exerciseId)
        {
            await _quizService.AddExerciseToQuiz(quizId, exerciseId);
            return RedirectToAction(
                nameof(ViewQuizzes),
                new { selectedQuizId = quizId }
            );
        }

        // POST /Quiz/RemoveExercise
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveExercise(int quizId, int exerciseId)
        {
            await _quizService.RemoveExerciseFromQuiz(quizId, exerciseId);
            return RedirectToAction(
                nameof(ViewQuizzes),
                new { selectedQuizId = quizId }
            );
        }

        // POST /Quiz/DeleteQuiz/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            await _quizService.DeleteQuiz(id);
            return RedirectToAction(nameof(ViewQuizzes));
        }

        // GET: /Quiz/CreateQuiz
        [HttpGet]
        public async Task<IActionResult> CreateQuiz()
        {
            TempData["SelectedExerciseIds"] = new List<int>();
            TempData.Keep("SelectedExerciseIds"); 

            await SetCreateQuizViewData(); 

            return View();
        }

        // POST: Add a selected exercise
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSelectedExercise(List<int> exerciseIds)
        {
            var selectedIds = TempData["SelectedExerciseIds"] as List<int> ?? new();
            foreach (var id in exerciseIds)
            {
                if (!selectedIds.Contains(id))
                    selectedIds.Add(id);
            }

            TempData["SelectedExerciseIds"] = selectedIds;
            TempData.Keep("SelectedExerciseIds");
            await SetCreateQuizViewData();
            return View("CreateQuiz");
        }

        // POST
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RemoveSelectedExercise(int exerciseId)
        {
            var selectedIds = TempData["SelectedExerciseIds"] as List<int> ?? new();
            selectedIds.Remove(exerciseId);

            TempData["SelectedExerciseIds"] = selectedIds;
            TempData.Keep("SelectedExerciseIds");

            return Json(new { success = true, remainingIds = selectedIds });
        }

        // POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuizConfirmed(List<int> SelectedExerciseIds)
        {
            if (SelectedExerciseIds == null || !SelectedExerciseIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one exercise.");
                await SetCreateQuizViewData();
                return View("CreateQuiz");
            }

            var quiz = new DuoClassLibrary.Models.Quizzes.Quiz(0, null, null);
            await _quizService.CreateQuiz(quiz); 

            var allQuizzes = await _quizService.GetAllQuizzes();
            int newQuizId = allQuizzes.Max(q => q.Id); 

            foreach (var id in SelectedExerciseIds)
                await _quizService.AddExerciseToQuiz(newQuizId, id);

            return RedirectToAction("ViewQuizzes", new { selectedQuizId = newQuizId });
        }


        private async Task SetCreateQuizViewData()
        {
            var allExercises = await _exerciseService.GetAllExercises();
            var selectedIds = TempData.Peek("SelectedExerciseIds") as List<int> ?? new();

            ViewBag.AvailableExercises = allExercises;
            TempData.Keep("SelectedExerciseIds");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableExercisesModal()
        {
            var allExercises = await _exerciseService.GetAllExercises();
            var selectedIds = TempData["SelectedExerciseIds"] as List<int> ?? new();
            TempData.Keep("SelectedExerciseIds");

            var available = allExercises.Where(e => !selectedIds.Contains(e.ExerciseId)).ToList();
            return PartialView("_AddExerciseModal", available);
        }

    }
}
