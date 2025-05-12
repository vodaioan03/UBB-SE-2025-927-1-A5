using Duo.Web.Models;
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

        private List<ExerciseViewModel> GetMockExercises()
        {
            return new List<ExerciseViewModel>
            {
                new ExerciseViewModel { Id = 1, Description = "Match the items for Association Exercise 1" },
                new ExerciseViewModel { Id = 2, Description = "Match the items for Association Exercise 2" },
                new ExerciseViewModel { Id = 3, Description = "Match the items for Association Exercise 4" },
                new ExerciseViewModel { Id = 4, Description = "Complete the sentence for Fill in the Blank Exercise 3" },
            };
                }

        public IActionResult ManageExam()
        {
            ViewBag.Exercises = GetMockExercises();
            return View();
        }

        public IActionResult AddExam()
        {
            ViewBag.Exercises = GetMockExercises();
            return View();
        }

    }
}
