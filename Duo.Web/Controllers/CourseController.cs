using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using Duo.Web.Models;
using System.Security.Claims;

namespace Duo.Web.Controllers
{
    public class CourseController(ICourseService courseService, ICoinsService coinsService) : Controller
    {
        public async Task<IActionResult> ViewCourses()
        {
            var courses = await courseService.GetCoursesAsync();
            return View(courses);
        }

        public async Task<IActionResult> CoursePreview(int id)
        {
            int userId = 1;  // Replace with actual userId if you're using authentication

            // Fetch course data
            var course = await courseService.GetCourseAsync(id);
            var modules = await courseService.GetModulesAsync(id);
            var isEnrolled = await courseService.IsUserEnrolledAsync(userId, id);
            var completedModules = await courseService.GetCompletedModulesCountAsync(userId, id);
            var requiredModules = await courseService.GetRequiredModulesCountAsync(id);
            var coinBalance = await coinsService.GetCoinBalanceAsync(userId);
            var tags = await courseService.GetCourseTagsAsync(id);

            // Prepare module view models
            var moduleViewModels = new List<ModuleViewModel>();

            foreach (var module in modules)
            {
                bool isUnlocked = await courseService.IsModuleAvailableAsync(userId, module.ModuleId);
                bool isCompleted = await courseService.IsModuleCompletedAsync(userId, module.ModuleId);

                // Add module details to the view model
                moduleViewModels.Add(new ModuleViewModel
                {
                    Module = module,
                    CourseId = id,  // Course ID is still relevant for context
                    IsCompleted = isCompleted,
                    IsUnlocked = isUnlocked,
                    TimeSpent = await courseService.GetTimeSpentAsync(userId, id).ContinueWith(t =>
                        TimeSpan.FromSeconds(t.Result).ToString(@"hh\:mm\:ss")), // Format time spent
                    CoinBalance = coinBalance.ToString()  // Assuming coin balance is an integer
                });
            }

            // Calculate the remaining time for the course (in seconds)
            int timeRemainingSeconds = await courseService.GetCourseTimeLimitAsync(id) - await courseService.GetTimeSpentAsync(userId, id);
            string formattedTimeRemaining = TimeSpan.FromSeconds(Math.Max(timeRemainingSeconds, 0)).ToString(@"hh\:mm\:ss");

            // Create the view model for the course preview
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