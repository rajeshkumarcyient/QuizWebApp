using QuizAppWeb.Models;
using System.Threading.Tasks;

namespace QuizAppWeb.Service
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserModel userModel);
        Task<bool> LoginAsync(LoginModel loginModel);
    }
}
