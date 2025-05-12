using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Quizzes;

namespace Duo.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly IQuizService _quizService;
        private readonly IExerciseService _exerciseService;


        public QuizController(ILogger<QuizController> logger, IQuizService quizService, IExerciseService exerciseService)
        {
            _logger = logger;
            _quizService = quizService;
            _exerciseService = exerciseService;
        }

        // GET: /Quiz/ViewQuizzes
        public async Task<IActionResult> ViewQuizzes()
        {
            List<Quiz> model = await _quizService.GetAllQuizzes();
            return View(model);
        }

        public IActionResult CreateQuiz()
        {
            return View();
        }

        // POST: /Quiz/DeleteQuiz/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            await _quizService.DeleteQuiz(id);
            return RedirectToAction(nameof(ViewQuizzes));
        }

        // GET: /Quiz/GetExercises?quizId=5
        [HttpGet]
        public async Task<IActionResult> GetExercises(int quizId)
        {
            var exercises = await _exerciseService.GetAllExercisesFromQuiz(quizId);
            if (exercises == null) return NotFound();

            var list = exercises
                .Select(e => new
                {
                    e.ExerciseId,
                    Question = e.Question,
                    Difficulty = e.Difficulty.ToString()
                })
                .ToList();

            return Json(list);
        }

    }
}