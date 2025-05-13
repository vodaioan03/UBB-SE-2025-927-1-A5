using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models.Quizzes;
using DuoClassLibrary.Models.Quizzes.API;
using DuoClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class ExamController : Controller
    {
        private readonly IExerciseServiceProxy _exerciseService;
        private readonly IQuizServiceProxy _quizService;

        public ExamController(IExerciseServiceProxy exerciseService, IQuizServiceProxy quizService)
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
        public async Task<IActionResult> CreateExam([FromBody] List<int> selectedExerciseIds)
        {
            var exam = new Exam(0, null);


            await _quizService.CreateExamAsync(exam);

            foreach (var id in selectedExerciseIds)
            {
                await _quizService.AddExerciseToExamAsync(exam.Id, id);
            }

            return RedirectToAction("ManageExam");
        }

        // GET: /Exam/ManageExam
        public async Task<IActionResult> ManageExam()
        {
            var exams = await _quizService.GetAllExams();
            var allExercises = await _exerciseService.GetAllExercises();

            ViewBag.Exams = exams;
            ViewBag.AllExercises = allExercises;

            return View();
        }

        // GET: /Exam/GetExercisesForExam/{id}
        [HttpGet]
        public async Task<IActionResult> GetExercisesForExam(int id)
        {
            var exercises = await _exerciseService.GetAllExercisesFromExam(id);
            return PartialView("_SelectedExercisesPartial", exercises);
        }

        // POST: /Exam/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _quizService.DeleteExamAsync(id);
            return RedirectToAction("ManageExam");
        }

        // POST: /Exam/RemoveExerciseFromExam
        [HttpPost]
        public async Task<IActionResult> RemoveExerciseFromExam([FromBody] ExamExerciseDto dto)
        {
            await _quizService.RemoveExerciseFromExamAsync(dto.ExamId, dto.ExerciseId);
            return Ok();
        }

        // POST: /Exam/AddExerciseToExam
        [HttpPost]
        public async Task<IActionResult> AddExerciseToExam([FromBody] ExamExerciseDto dto)
        {
            await _quizService.AddExerciseToExamAsync(dto.ExamId, dto.ExerciseId);
            return Ok();
        }

        // POST: /Exam/RenderSelectedExercises
        [HttpPost]
        public IActionResult RenderSelectedExercises([FromBody] List<ExerciseDto> selectedExercises)
        {
            var proxyExercises = selectedExercises
                .Select(dto => new MultipleChoiceExercise
                {
                    ExerciseId = dto.ExerciseId,
                    Question = dto.Question
                })

                .ToList();

            return PartialView("_SelectedExercisesPartial", proxyExercises);
        }
    }
}
