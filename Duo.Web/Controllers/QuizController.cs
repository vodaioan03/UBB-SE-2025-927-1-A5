// QuizController.cs

using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Models.Exercises;
using System.Linq;
using System.Threading.Tasks;
using Duo.Web.Models;

namespace Duo.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IExerciseService _exerciseService;

        public QuizController(IQuizService quizService,
                              IExerciseService exerciseService)
        {
            _quizService = quizService;
            _exerciseService = exerciseService;
        }

        // GET /Quiz/ViewQuizzes?selectedQuizId=5
        [HttpGet]
        public async Task<IActionResult> ViewQuizzes(int? selectedQuizId)
        {
            var vm = new ManageQuizViewModel
            {
                Quizzes = await _quizService.GetAllQuizzes(),
                SelectedQuizId = selectedQuizId
            };

            if (selectedQuizId.HasValue && selectedQuizId > 0)
            {
                try
                {
                    vm.AssignedExercises = await _exerciseService
                   .GetAllExercisesFromQuiz(selectedQuizId.Value);

                    var all = await _exerciseService.GetAllExercises();
                    vm.AvailableExercises = all
                       .Where(e => !vm.AssignedExercises
                                      .Any(a => a.ExerciseId == e.ExerciseId))
                       .ToList();
                }
                catch (HttpRequestException ex)
                {
                    // Optional: Log the error
                    Console.WriteLine($"Failed to fetch exercises for quiz ID {selectedQuizId}: {ex.Message}");
                    vm.AssignedExercises = new List<Exercise>();
                }
            }
            return View(vm);
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

            return Ok(); 
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

            var quiz = new Quiz(0, null, null);
            int newQuizId = await _quizService.CreateQuiz(quiz);

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
