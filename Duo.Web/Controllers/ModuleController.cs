using Duo.Web.Models;
using DuoClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public ModulesController(ICourseService courseService, IUserService userService)
        {
            _courseService = courseService;
            _userService = userService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var module = await _courseService.GetModuleByIdAsync(id);
            var userId = await _userService.GetCurrentUserIdAsync();

            var viewModel = new ModuleViewModel
            {
                Module = module,
                CourseId = module.CourseId,
                TimeSpent = await _userService.GetTimeSpentOnModule(userId, id),
                CoinBalance = await _userService.GetCoinBalance(userId),
                IsCompleted = await _userService.HasCompletedModule(userId, id)
            };

            return View("Module", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            await _courseService.MarkModuleAsCompleted(userId, id);
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult ImageClick(int id)
        {
            // Redirect or handle modal/view for image click
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
