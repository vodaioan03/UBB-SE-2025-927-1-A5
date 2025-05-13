using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using Duo.Web.Models;
using System.Security.Claims;

namespace Duo.Web.Controllers
{
    public class CourseController(ICourseService courseService) : Controller
    {
        public async Task<IActionResult> ViewCourses()
        {
            var courses = await courseService.GetCoursesAsync();
            return View(courses);
        }

        public async Task<IActionResult> CoursePreview(int id)
        {
            // Assume you're using claims to get the authenticated user ID
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var course = await courseService.GetCourseAsync(id);
            var modules = await courseService.GetModulesAsync(id);
            var isEnrolled = await courseService.IsUserEnrolledAsync(userId, id);
            var completedModules = await courseService.GetCompletedModulesCountAsync(userId, id);
            var requiredModules = await courseService.GetRequiredModulesCountAsync(id);
            var coinBalance = await userService.GetCoinBalanceAsync(userId); // Assuming your userService supports this
            var tags = await courseService.GetCourseTagsAsync(id);

            var moduleViewModels = new List<ModuleViewModel>();

            foreach (var module in modules)
            {
                bool isUnlocked = await courseService.IsModuleAvailableAsync(userId, module.Id);
                bool isCompleted = await courseService.IsModuleCompletedAsync(userId, module.Id);

                moduleViewModels.Add(new ModuleViewModel
                {
                    Module = module,
                    IsBonus = module.IsBonus,
                    IsUnlocked = isUnlocked,
                    IsCompleted = isCompleted
                });
            }

            int timeRemainingSeconds = await courseService.GetCourseTimeLimitAsync(id) - await courseService.GetTimeSpentAsync(userId, id);
            string formattedTimeRemaining = TimeSpan.FromSeconds(Math.Max(timeRemainingSeconds, 0)).ToString(@"hh\:mm\:ss");

            var viewModel = new CoursePreviewViewModel
            {
                Course = course,
                Modules = moduleViewModels,
                IsEnrolled = isEnrolled,
                CompletedModules = completedModules,
                RequiredModules = requiredModules,
                CoinBalance = coinBalance,
                Tags = tags,
                FormattedTimeRemaining = formattedTimeRemaining
            };

            return View("ViewCoursePreview", viewModel);
        }
    }
}