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

        public QuizController(ILogger<QuizController> logger, IQuizService quizService)
        {
            _logger = logger;
            _quizService = quizService;
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

    }
}
