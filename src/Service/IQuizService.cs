using QuizAppWeb.Models;
using QuizAppWeb.ViewModel;
using System.Net;

namespace QuizAppWeb.Service
{
    public interface IQuizService
    {
        Task<HttpStatusCode> CreateQuiz(QuizViewModel quizViewModel);

        Task<List<Quiz>> GetAllQuizzesAsync();

        Task<QuizViewModel> GetQuizById(int quizId);

        Task<HttpStatusCode> UpdateQuiz(QuizViewModel quizViewModel);

        Task<HttpStatusCode> DeleteQuiz(int quizId);


    }
}
