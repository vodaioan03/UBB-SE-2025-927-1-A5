// Controllers/ExerciseController.cs
using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers;
public class ExerciseController : Controller
{
    private readonly ILogger<ExerciseController> _logger;
    private readonly IExerciseService _exerciseService;

    public ExerciseController(ILogger<ExerciseController> logger, IExerciseService exerciseService)
    {
        _logger = logger;
        _exerciseService = exerciseService;
    }

    public IActionResult Index()
    {
        // Return the index view that lists all exercises
        return View("~/Views/Exercise/Index.cshtml");
    }

    public IActionResult AssociationExercise()
    {
        // Using mock data in the view for now
        return View("~/Views/Exercise/AssociationExercise.cshtml");
    }

    public IActionResult FillBlanksExercise()
    {
        // Using mock data in the view for now
        return View("~/Views/Exercise/FillBlanksExercise.cshtml");
    }

    public IActionResult MultipleChoiceExercise()
    {
        // Using mock data in the view for now
        return View("~/Views/Exercise/MultipleChoiceExercise.cshtml");
    }

    public IActionResult FlashcardExercise()
    {
        // Using mock data in the view for now
        return View("~/Views/Exercise/FlashcardExercise.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitExercise(int exerciseId, string selectedChoice, List<string> answers, List<AssociationPair> pairs, string answer)
    {
        var exercise = await _exerciseService.GetExerciseById(exerciseId);
        bool isCorrect = false;

        switch (exercise.Type)
        {
            case "MultipleChoice":
                var mcModel = exercise as MultipleChoiceExercise;
                isCorrect = mcModel.ValidateAnswer(new List<string> { selectedChoice });
                return Content($"Your answer: {selectedChoice}, Correct: {isCorrect}");

            case "FillInTheBlank":
                var fibModel = exercise as FillInTheBlankExercise;
                isCorrect = fibModel.ValidateAnswer(answers);
                return Content($"Your answers: {string.Join(", ", answers)}, Correct: {isCorrect}");

            case "Flashcard":
                var fcModel = exercise as FlashcardExercise;
                isCorrect = fcModel.ValidateAnswer(answer);
                return Content($"Your answer: {answer}, Correct: {isCorrect}");

            case "Association":
                var assocModel = exercise as AssociationExercise;
                var userPairs = pairs.Select(p => (p.Item, p.Match)).ToList();
                isCorrect = assocModel.ValidateAnswer(userPairs);
                return Content($"Your pairs: {string.Join(", ", pairs.Select(p => $"{p.Item} -> {p.Match}"))}, Correct: {isCorrect}");

            default:
                return Content("Unsupported exercise type.");
        }
    }
}

public class AssociationPair
{
    public string Item { get; set; }
    public string Match { get; set; }
}