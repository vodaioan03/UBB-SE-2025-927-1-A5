using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Sections;
using System.Threading.Tasks;

namespace Duo.Web.Controllers
{
    public class SectionController : Controller
    {
        private readonly ILogger<SectionController> _logger;
        private readonly ISectionService _sectionService;

        public SectionController(ILogger<SectionController> logger, ISectionService sectionService)
        {
            _logger = logger;
            _sectionService = sectionService;
        }

        public async Task<IActionResult> ManageSection()
        {
            var sections = await _sectionService.GetAllSections();
            return View(sections);
        }

        public IActionResult AddSection()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSection(Section section)
        {
            if (!ModelState.IsValid)
            {
                return View("AddSection", section);
            }

            await _sectionService.AddSection(section);
            return RedirectToAction("ManageSection");
        }

        [HttpGet]
        public async Task<IActionResult> EditSection(int sectionId)
        {
            var section = await _sectionService.GetSectionById(sectionId);
            return View(section);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSection(Section section)
        {
            if (!ModelState.IsValid)
            {
                return View("EditSection", section);
            }

            await _sectionService.UpdateSection(section);
            return RedirectToAction("ManageSection");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            await _sectionService.DeleteSection(sectionId);
            return RedirectToAction("ManageSection");
        }
    }
}
