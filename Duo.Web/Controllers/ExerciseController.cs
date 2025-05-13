using Duo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Models;
using DuoClassLibrary.Models.Exercises;
using System.Threading.Tasks;
using DuoClassLibrary.Services;
using System.Text.Json;

namespace DuoWebApp.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IExerciseService exerciseService;

        public ExerciseController(IExerciseService exerciseService)
        {
            this.exerciseService = exerciseService;
        }

        public IActionResult CreateExercise()
        {
            ViewBag.Difficulties = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToArray();
            ViewBag.ExerciseTypes = ExerciseTypes.EXERCISE_TYPES;
            return View("~/Views/Exercise/CreateExercise.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement exerciseJson)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors as JSON for AJAX
                return BadRequest(ModelState);
            }

            string json = exerciseJson.GetRawText();

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("Type", out var typeProp))
            {
                return BadRequest("Missing 'type' discriminator.");
            }

            var type = typeProp.GetString();

            Exercise? exercise = type switch
            {
                "Flashcard" => JsonSerializer.Deserialize<FlashcardExercise>(json),
                "MultipleChoice" => JsonSerializer.Deserialize<MultipleChoiceExercise>(json),
                "FillInTheBlank" => JsonSerializer.Deserialize<FillInTheBlankExercise>(json),
                "Association" => JsonSerializer.Deserialize<AssociationExercise>(json),
                _ => null
            };

            if (exercise == null)
            {
                return this.BadRequest("Invalid payload.");
            }

            await exerciseService.CreateExercise(exercise);

            // Return success (could also return the created entity or a redirect URL)
            return Ok(new { message = "Exercise created successfully" });
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
            var exercises = await exerciseService.GetAllExercises();
            return View("~/Views/Exercise/ManageExercise.cshtml", exercises);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await exerciseService.DeleteExercise(id);
            return RedirectToAction("Manage");
        }
    }
}