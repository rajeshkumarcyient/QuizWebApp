using Microsoft.AspNetCore.Mvc;

namespace QuizAppWeb.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
