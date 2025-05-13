using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class ExamController : Controller
    {
        private readonly IExerciseService _exerciseService;
        private readonly IQuizService _quizService;

        public ExamController(IExerciseService exerciseService, IQuizService quizService)
        {
            _exerciseService = exerciseService;
            _quizService = quizService;
        }

        // GET: /Exam/AddExam
        public async Task<IActionResult> AddExam()
        {
            var allExercises = await _exerciseService.GetAllExercises();
            ViewBag.AllExercises = allExercises;
            return View();
        }



        // POST: /Exam/CreateExam
        [HttpPost]
        public async Task<IActionResult> CreateExam(List<int> selectedExerciseIds)
        {
            var exam = new Exam(0, null); // or pass a sectionId if required
            int examId = await _quizService.CreateExam(exam);

            foreach (var id in selectedExerciseIds)
            {
                await _quizService.AddExerciseToExam(examId, id);
            }

            return RedirectToAction("ManageExam");
        }

        // GET: /Exam/ManageExam
        public async Task<IActionResult> ManageExam()
        {
            var exams = await _quizService.GetAllExams();
            var allExercises = await _exerciseService.GetAllExercises(); // <-- this line is new

            ViewBag.Exams = exams;
            ViewBag.AllExercises = allExercises; // <-- so that the modal can list all exercises to add

            return View();
        }



        // GET: /Exam/GetExercisesForExam/5
        [HttpGet]
        public async Task<IActionResult> GetExercisesForExam(int id)
        {
            var exercises = await _exerciseService.GetAllExercisesFromExam(id);
            return PartialView("_SelectedExercisesPartial", exercises);
        }


        // POST: /Exam/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _quizService.DeleteExam(id);
            return RedirectToAction("ManageExam");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExerciseFromExam([FromBody] ExamExerciseDto dto)
        {
            await _quizService.RemoveExerciseFromExam(dto.ExamId, dto.ExerciseId);
            return Ok();
        }

        [HttpPost]
        public IActionResult RenderSelectedExercises([FromBody] List<ExerciseDto> selectedExercises)
        {
            var exercises = selectedExercises
                .Select(dto => new MultipleChoiceExercise
                {
                    ExerciseId = dto.ExerciseId,
                    Question = dto.Question
                })
                .Cast<Exercise>()
                .ToList();

            return PartialView("_SelectedExercisesPartial", exercises);
        }



    }
}
