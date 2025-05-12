// QuizController.cs

using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Models.Exercises;
using System.Linq;
using System.Threading.Tasks;

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

            if (selectedQuizId.HasValue)
            {
                vm.AssignedExercises = await _exerciseService
                   .GetAllExercisesFromQuiz(selectedQuizId.Value);

                var all = await _exerciseService.GetAllExercises();
                vm.AvailableExercises = all
                   .Where(e => !vm.AssignedExercises
                                  .Any(a => a.ExerciseId == e.ExerciseId))
                   .ToList();
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
    }
}
