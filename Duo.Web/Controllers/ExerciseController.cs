using System.Diagnostics;
using Duo.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly ILogger<ExerciseController> _logger;
        public ExerciseController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
        }
        // GET: Exercise
        // This is just a placeholder to show the types of exercise components from Views/Exercise

        // Just show the types type of exercise components from Views/Exercise

        public IActionResult Index()
        {
            // URL : /exercise
            return View("~/Views/Exercise/AssociationExercise.cshtml");
        }
        public IActionResult AssociationExercise()
        {
            // URL : /exercise/AssociationExercise
            return View("~/Views/Exercise/AssociationExercise.cshtml");
        }
        public IActionResult FillInTheBlank()
        {
            // URL : /exercise/FillInTheBlank
            return View("~/Views/Exercise/FillInTheBlank.cshtml");
        }
        public IActionResult MultipleChoice()
        {
            // URL : /exercise/MultipleChoice
            return View("~/Views/Exercise/MultipleChoice.cshtml");
        }
        public IActionResult FillBlanks()
        {
            // URL : /exercise/FillBlanks
            return View("~/Views/Exercise/FillBlanks.cshtml");
        }
    }
}
