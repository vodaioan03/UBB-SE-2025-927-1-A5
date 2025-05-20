using Duo.Web.ViewModels;
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
        private readonly ISectionService _sectionService;


        public ExamController(IExerciseServiceProxy exerciseService, IQuizServiceProxy quizService, ISectionService sectionService)
        {
            _exerciseService = exerciseService;
            _quizService = quizService;
            _sectionService = sectionService;
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
            var newExam = new Exam(0, null);
            Exam createdExam = await _quizService.CreateExamAsync(newExam);

            var allExams = await _quizService.GetAllExams();
            int newExamId = allExams.Max(q => q.Id);

            foreach (var id in selectedExerciseIds)
            {
                await _quizService.AddExerciseToExamAsync(newExamId, id);
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
                    Question = dto.Question,
                    Type = "MultipleChoice"
                })
                .ToList<Exercise>();

            return PartialView("_SelectedExercisesPartial", proxyExercises);
        }

        // GET: Exam/Preview/{id}
        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            try
            {
                var allExams = await _quizService.GetAllExams();
                var exam = allExams.FirstOrDefault(e => e.Id == id);

                if (exam != null)
                {
                    var vm = new ExamPreviewViewModel
                    {
                        Exam = exam,
                        ExerciseIds = exam.Exercises.Select(e => e.ExerciseId).ToList(),
                        SectionTitle = exam.SectionId.HasValue
                            ? (await _sectionService.GetSectionById(exam.SectionId.Value))?.Title
                              ?? "Section:"
                            : "Section:"
                    };
                    return View("ExamPreview", vm);
                }

                var currentExam = await _quizService.GetExamByIdAsync(id);
                if (currentExam == null) return NotFound();

                var viewModel = new ExamPreviewViewModel
                {
                    Exam = currentExam,
                    ExerciseIds = currentExam.Exercises.Select(e => e.ExerciseId).ToList(),
                    SectionTitle = currentExam.SectionId.HasValue
                        ? (await _sectionService.GetSectionById(currentExam.SectionId.Value))?.Title
                          ?? "Section:"
                        : "Section:"
                };

                return View("ExamPreview", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Preview: {ex.Message}");
                throw;
            }
        }
    }
}
