// Controllers/RoadmapController.cs

using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using DuoClassLibrary.Models.Sections;
using Duo.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Duo.Web.Controllers
{
    public class RoadmapController : Controller
    {
        private readonly IRoadmapService _roadmapService;
        private readonly IUserService _userService;
        private readonly ISectionService _sectionService;

        public RoadmapController(
            IRoadmapService roadmapService,
            IUserService userService,
            ISectionService sectionService)
        {
            _roadmapService = roadmapService;
            _userService = userService;
            _sectionService = sectionService;
        }

        // GET /Roadmap/ViewRoadmap?roadmapId=1&userId=1
        [HttpGet]
        public async Task<IActionResult> ViewRoadmap(int roadmapId, int userId)
        {
            var roadmap = await _roadmapService.GetByIdAsync(roadmapId);
            var user = await _userService.GetByIdAsync(userId);
            var sections = await _sectionService.GetByRoadmapId(roadmap.Id);

            var vm = new ManageRoadmapViewModel
            {
                RoadmapName = roadmap.Name,
                NumberOfCompletedSections = user.NumberOfCompletedSections,
                Sections = new List<SectionViewModel>()
            };

            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                var isUnlocked = i <= user.NumberOfCompletedSections;
                var completedQuizzes = i == user.NumberOfCompletedSections
                    ? user.NumberOfCompletedQuizzesInSection
                    : (isUnlocked ? 0 : -1);

                vm.Sections.Add(new SectionViewModel
                {
                    SectionId = section.Id,
                    Title = section.Title,
                    IsUnlocked = isUnlocked,
                    CompletedQuizzes = completedQuizzes
                });
            }

            return View(vm);
        }
    }
}
