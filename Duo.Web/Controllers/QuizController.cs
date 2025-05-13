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

        [HttpGet("Quiz/{id}/Solve")] // Or [Route("Quiz/{id}/Solve")]
        public async Task<IActionResult> Solve(int id)
        {
            var quiz = await _quizService.GetQuizById(id);

            if (quiz == null || !quiz.Exercises.Any())
            {
                if (quiz == null || quiz.Exercises == null || !quiz.Exercises.Any())
                {
                    return NotFound("Quiz not found or has no exercises.");
                }
            }

            var viewModel = new QuizSolverViewModel
            {
                QuizId = quiz.Id,
                QuizTitle = "Quiz" + quiz.Id.ToString(),
                AllExercises = quiz.Exercises,
                CurrentExerciseIndex = 0
            };

            if (viewModel.AllExercises.Any())
            {
                viewModel.CurrentExercise = viewModel.AllExercises.First();
                viewModel.CurrentExerciseType = viewModel.CurrentExercise.GetType().Name;
            }
            else
            {
                return View("NoExercises");
            }

            viewModel.IsLastExercise = viewModel.AllExercises.Count == 1;

            return View("Solve", viewModel);
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
