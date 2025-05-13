using Duo.Web.Models;
using DuoClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly ICoinsService _coinsService;

        public ModuleController(ICourseService courseService, IUserService userService, ICoinsService coinsService)
        {
            _courseService = courseService;
            _userService = userService;
            _coinsService = coinsService;
        }

        /*public async Task<IActionResult> Details(int id)
        {
            // Fetch the module details
            var module = await _courseService.GetModuleByIdAsync(id);
            var userId = await _userService.GetCurrentUserIdAsync();

            // Get the necessary details
            var timeSpent = await _userService.GetTimeSpentOnModule(userId, id);
            var coinBalance = await _coinsService.GetCoinBalanceAsync(userId);
            var isCompleted = await _userService.HasCompletedModule(userId, id);

            // Create the view model with the required properties
            var viewModel = new ModuleViewModel
            {
                Module = module,
                CourseId = module.CourseId, // Get course ID from the module
                TimeSpent = TimeSpan.FromSeconds(timeSpent).ToString(@"hh\:mm\:ss"), // Format time spent
                CoinBalance = coinBalance.ToString(), // Assuming coin balance is an integer
                IsCompleted = isCompleted,
                IsUnlocked = await _courseService.IsModuleAvailableAsync(userId, id) // Determine if the module is unlocked
            };

            // Return the view with the populated view model
            return View("Module", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            await _courseService.MarkModuleAsCompleted(userId, id);
            return RedirectToAction(nameof(Details), new { id });
        }*/

        /*public IActionResult ImageClick(int id)
        {
            // Handle image click, can be redirected or open a modal
            return RedirectToAction(nameof(Details), new { id });
        }*/
        //[Route("Module/{id:int}")]
        public IActionResult Index()
        {
            return View("~/Views/Module/Index.cshtml");
        }
    }
}
