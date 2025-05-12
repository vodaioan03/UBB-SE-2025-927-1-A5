using Duo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Models;
using DuoClassLibrary.Models.Exercises;
using System.Threading.Tasks;

namespace DuoWebApp.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly ILogger<ExerciseController> _logger;

        public ExerciseController(ILogger<ExerciseController> logger)
        {
            _logger = logger;
        }

        public IActionResult CreateExercise()
        {
            ViewBag.Difficulties = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToArray();
            ViewBag.ExerciseTypes = ExerciseTypes.EXERCISE_TYPES;
            return View("~/Views/Exercise/Create.cshtml");
        }

        public IActionResult ViewExercises()
        {
            return View("~/Views/Exercise/ViewExercises.cshtml");
        }

        [HttpGet]
        public IActionResult GetExerciseForm(string type)
        {
            switch (type)
            {
                case "Association":
                    return PartialView("_AssociationExerciseForm");
                case "Fill in the blank":
                    return PartialView("_FillInTheBlankExerciseForm");
                case "Multiple Choice":
                    return PartialView("_MultipleChoiceExerciseForm");
                case "Flashcard":
                    return PartialView("_FlashcardExerciseForm", new FlashcardExercise());
                default:
                    return NotFound();
            }
        }
    }
}