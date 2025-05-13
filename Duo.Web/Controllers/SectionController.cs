using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Sections;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            List<Section> sections = await _sectionService.GetAllSections();
            return View("~/Views/Section/ManageSection.cshtml", sections);
        }

        public IActionResult AddSection()
        {
            return View("~/Views/Section/AddSection.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddSection(Section section)
        {
            if (!ModelState.IsValid)
                return View(section);

            await _sectionService.AddSection(section);
            return RedirectToAction("ManageSection");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSection(int id)
        {
            await _sectionService.DeleteSection(id);
            return RedirectToAction("ManageSection");
        }

        public async Task<IActionResult> EditSection(int id)
        {
            var section = await _sectionService.GetSectionById(id);
            if (section == null)
                return NotFound();
            return View("~/Views/Section/EditSection.cshtml", section);
        }

        [HttpPost]
        public async Task<IActionResult> EditSection(Section section)
        {
            if (!ModelState.IsValid)
                return View(section);
            await _sectionService.UpdateSection(section);
            return RedirectToAction("ManageSection");
        }
    }
}