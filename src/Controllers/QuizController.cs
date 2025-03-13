using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizAppWeb.Service;
using QuizAppWeb.ViewModel;
using System.Reflection;

namespace QuizAppWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public async Task<IActionResult> CreateOrEdit()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();

            var viewModel = new QuizViewModel
            {
                Quizzes = quizzes
            };

            return View("CreateOrEdit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {
                await _quizService.CreateQuiz(quizViewModel);
                return RedirectToAction("CreateOrEdit");
            }

            // Reload the quizzes if validation fails
            quizViewModel.Quizzes = await _quizService.GetAllQuizzesAsync();
            return View("CreateOrEdit", quizViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _quizService.GetQuizById(id);
            if (quiz == null)
            {
                return NotFound();
            }

            var viewModel = new QuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                DurationMinutes = quiz.DurationMinutes,
                CreatedBy = quiz.CreatedBy,
                CreatedAt = quiz.CreatedAt,
                Quizzes = await _quizService.GetAllQuizzesAsync()
            };

            return View("CreateOrEdit", viewModel);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(QuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _quizService.UpdateQuiz(model);
                return RedirectToAction("CreateOrEdit");
            }

            model.Quizzes = await _quizService.GetAllQuizzesAsync();
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _quizService.DeleteQuiz(id);
            return RedirectToAction("CreateOrEdit");
        }


    }
}
