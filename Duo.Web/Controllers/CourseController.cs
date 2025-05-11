using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult ViewCourses()
        {
            return View();
        }
    }
}
