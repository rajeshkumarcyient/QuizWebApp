using QuizAppWeb.Models;
using QuizAppWeb.Service;

namespace QuizAppWeb.Service
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;
        private const string BaseUrl = "https://localhost:44362/Auth";

        public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<bool> RegisterUserAsync(UserModel userModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", userModel);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogWarning("User registration failed. Status Code: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                return false;
            }
        }

        public async Task<bool> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", loginModel);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogWarning("Login failed. Status Code: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login.");
                return false;
            }
        }
    }
}

