using Microsoft.AspNetCore.Mvc;
using QuizAppWeb.Service;
using QuizAppWeb.ViewModel;
using System.Reflection;

namespace QuizAppWeb.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {   
                _quizService.CreateQuiz(quizViewModel);
                return RedirectToAction("Create");
            }

            return View();
        }


    }
}
