using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services;
using Duo.Web.Models;
using System.Security.Claims;
using DuoClassLibrary.Models;
using System.Text.Json;

namespace Duo.Web.Controllers
{
    public class CourseController(ICourseService courseService, ICoinsService coinsService) : Controller
    {
        public async Task<IActionResult> ViewCourses()
        {
            var courses = await courseService.GetCoursesAsync();
            foreach (var course in courses)
            {
                course.Tags = await courseService.GetCourseTagsAsync(course.CourseId);
            }
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await courseService.GetTagsAsync();
            return Json(tags);
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
                bool isUnlocked = await GetModuleUnlockStatus(module, userId);
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

        private async Task<bool> GetModuleUnlockStatus(Module module, int currentUserId)
        {
            var modules = await courseService.GetModulesAsync(module.CourseId);
            int moduleIndex = 0;
            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].ModuleId == module.ModuleId)
                {
                    moduleIndex = i;
                    break;
                }
            }
            try
            {
                var IsEnrolled = await courseService.IsUserEnrolledAsync(currentUserId, module.CourseId);
                if (!module.IsBonus)
                {
                    return IsEnrolled &&
                           (moduleIndex == 0 ||
                            await courseService.IsModuleCompletedAsync(currentUserId, modules[moduleIndex - 1].ModuleId));
                }
                return await courseService.IsModuleInProgressAsync(currentUserId, module.ModuleId);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<IActionResult> Enroll(int id)
        {
            int userId = 1;
            var CurrentCourse = await courseService.GetCourseAsync(id);
            var CoinBalance = await coinsService.GetCoinBalanceAsync(userId);
            var IsEnrolled = await courseService.IsUserEnrolledAsync(userId, id);

            if (!IsEnrolled && (CurrentCourse.Cost == 0 || CoinBalance >= CurrentCourse.Cost))
            {
                if (CurrentCourse.Cost > 0)
                {
                    bool coinDeductionSuccessful = await coinsService.TrySpendingCoinsAsync(userId, CurrentCourse.Cost);
                }

                bool enrollmentSuccessful = await courseService.EnrollInCourseAsync(userId, CurrentCourse.CourseId);

                CoinBalance = await coinsService.GetCoinBalanceAsync(userId);
                IsEnrolled = true;

                // Start Course Progress Timer ...
            }

            return RedirectToRoute(
                new
                {
                    controller = "Course",
                    action = "CoursePreview",
                    id = CurrentCourse.CourseId
                });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTimeLocal([FromBody] Dictionary<string, object> data)
        {
            // Extract values safely
            if (!data.TryGetValue("userId", out var userIdObj) ||
                !data.TryGetValue("courseId", out var courseIdObj) ||
                !data.TryGetValue("seconds", out var secondsObj))
            {
                return BadRequest("Missing one or more required parameters: userId, courseId, or seconds.");
            }

            // Extract from JsonElement
            int userId, courseId, seconds;

            if (userIdObj is JsonElement userIdElement)
            {
                userId = userIdElement.ValueKind == JsonValueKind.Number
                    ? userIdElement.GetInt32()
                    : int.Parse(userIdElement.GetString());
            }
            else
            {
                userId = Convert.ToInt32(userIdObj);
            }

            if (courseIdObj is JsonElement courseIdElement)
            {
                courseId = courseIdElement.ValueKind == JsonValueKind.Number
                    ? courseIdElement.GetInt32()
                    : int.Parse(courseIdElement.GetString());
            }
            else
            {
                courseId = Convert.ToInt32(courseIdObj);
            }

            if (secondsObj is JsonElement secondsElement)
            {
                seconds = secondsElement.ValueKind == JsonValueKind.Number
                    ? secondsElement.GetInt32()
                    : int.Parse(secondsElement.GetString());
            }
            else
            {
                seconds = Convert.ToInt32(secondsObj);
            }

            await courseService.UpdateTimeSpentAsync(userId, courseId, seconds);
            return Ok();
        }
    }
}