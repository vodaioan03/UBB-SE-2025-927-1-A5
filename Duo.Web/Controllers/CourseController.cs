using Microsoft.AspNetCore.Mvc;
using DuoClassLibrary.Services.Interfaces;
using DuoClassLibrary.Services;

namespace Duo.Web.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService courseService;

        public CourseController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public async Task<IActionResult> ViewCourses()
        {
            var courses = await courseService.GetCoursesAsync();
            return View(courses);
        }
    }
}
