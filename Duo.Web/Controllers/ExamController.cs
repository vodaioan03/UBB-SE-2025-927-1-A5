using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Duo.Web.Controllers
{
    public class ExamController : Controller
    {
        private readonly ILogger<ExamController> _logger;

        public ExamController(ILogger<ExamController> logger)
        {
            _logger = logger;
        }

        public IActionResult ManageExam()
        {
            return View("~/Views/Exam/ManageExam.cshtml");
        }

        public IActionResult AddExam()
        {
            return View("~/Views/Exam/AddExam.cshtml");
        }
    }
}
