using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DuoClassLibrary.Services;
using System.Threading.Tasks;

namespace Duo.Web.Controllers
{
    public class SectionController : Controller
    {
        private readonly ILogger<SectionController> _logger;
        private readonly ISectionService _sectionService;
        private readonly IQuizService _quizService;

        public SectionController(ILogger<SectionController> logger, ISectionService sectionService, IQuizService quizService)
        {
            _logger = logger;
            _sectionService = sectionService;
            _quizService = quizService;
        }

        public IActionResult ManageSection()
        {
            return View("~/Views/Section/ManageSection.cshtml");
        }

        public IActionResult AddSection()
        {
            return View("~/Views/Section/AddSection.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSections()
        {
            var sections = await _sectionService.GetAllSections();
            return Json(sections);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizzesForSection(int sectionId)
        {
            var quizzes = await _quizService.GetAllQuizzesFromSection(sectionId);
            return Json(quizzes);
        }
    }
}