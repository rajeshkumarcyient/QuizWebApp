using QuizAppWeb.Models;
using QuizAppWeb.ViewModel;
using System.Net;

namespace QuizAppWeb.Service
{
    public interface IQuizService
    {
        Task<HttpStatusCode> CreateQuiz(QuizViewModel quizViewModel);
    }
}
