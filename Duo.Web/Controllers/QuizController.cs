using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Duo.Web.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;

        public QuizController(ILogger<QuizController> logger)
        {
            _logger = logger;
        }

        public IActionResult ViewQuizzes()
        {
            return View(); 
        }

        public IActionResult Index()
        {
            return RedirectToAction("ViewQuizzes");
        }

        public IActionResult Manage()
        {
            return View("ViewQuizzes"); 
        }

    }
}
