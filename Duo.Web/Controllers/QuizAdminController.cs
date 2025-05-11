using Microsoft.AspNetCore.Mvc;

public class QuizAdminController : Controller
{
    public IActionResult Index()
    {
        return View(); 
    }

    public IActionResult ManageQuizzes()
    {
        return RedirectToAction("Manage", "Quiz"); 
    }
}
