using Duo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Models;
using DuoClassLibrary.Models.Exercises;
using System.Threading.Tasks;
using DuoClassLibrary.Services;

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
            return View("~/Views/Exercise/CreateExercise.cshtml");
        }

        public IActionResult ViewExercises()
        {
            return View("~/Views/Exercise/ManageExercise.cshtml");
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

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            //var exercises = await _exerciseService.GetAllExercises();
            var exercises = new List<Exercise>
            {
                new FlashcardExercise(1, "What is the capital of France?", "Paris", Difficulty.Easy)
                {
                    Type = "Flashcard"
                },
                new MultipleChoiceExercise(
                    2,
                    "Which planet is known as the Red Planet?",
                    Difficulty.Normal,
                    new List<MultipleChoiceAnswerModel>
                    {
                        new MultipleChoiceAnswerModel("Earth", false),
                        new MultipleChoiceAnswerModel("Mars", true), // Mars is correct
                        new MultipleChoiceAnswerModel("Jupiter", false),
                        new MultipleChoiceAnswerModel("Venus", false)
                    }
                )
                {
                    Type = "Multiple Choice"
                },
                new AssociationExercise(
                    3,
                    "Match the countries to their capitals.",
                    Difficulty.Hard,
                    new List<string> { "France", "Germany" },
                    new List<string> { "Paris", "Berlin" }
                )
                {
                    Type = "Association"
                }
            };
            return View("~/Views/Exercise/ManageExercise.cshtml", exercises);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //await _exerciseService.DeleteExercise(id);
            return RedirectToAction("ManageExercise");
        }

        [HttpGet]
        public async Task<IActionResult> GetExerciseDetails(int id)
        {
            //var exercise = await _exerciseService.GetExerciseById(id);
            Exercise exercise = id switch
            {
                1 => new FlashcardExercise(1, "What is the capital of France?", "Paris", Difficulty.Easy)
                {
                    Type = "Flashcard"
                },
                2 => new MultipleChoiceExercise(
                    2,
                    "Which planet is known as the Red Planet?",
                    Difficulty.Normal,
                    new List<MultipleChoiceAnswerModel>
                    {
                        new MultipleChoiceAnswerModel("Earth", false),
                        new MultipleChoiceAnswerModel("Mars", true), // Mars is correct
                        new MultipleChoiceAnswerModel("Jupiter", false),
                        new MultipleChoiceAnswerModel("Venus", false)
                    }
                )
                {
                    Type = "Multiple Choice"
                },
                3 => new AssociationExercise(
                    3,
                    "Match the countries to their capitals.",
                    Difficulty.Hard,
                    new List<string> { "France", "Germany" },
                    new List<string> { "Paris", "Berlin" }
                )
                {
                    Type = "Association"
                },
                _ => null
            };

            if (exercise == null)
                return NotFound();

            return PartialView("_ExerciseDetails", exercise);
        }
    }
}