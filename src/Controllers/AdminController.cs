using Microsoft.AspNetCore.Mvc;

namespace QuizAppWeb.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard() => View();
        public IActionResult Index()
        {
            return View();
        }
    }
}
