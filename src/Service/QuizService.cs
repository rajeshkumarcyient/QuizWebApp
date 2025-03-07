using Microsoft.AspNetCore.Http;
using QuizAppWeb.Models;
using QuizAppWeb.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace QuizAppWeb.Service
{
    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuizService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string BaseUrl = "https://localhost:44362/Quiz";

        public QuizService(IHttpClientFactory httpClientFactory, ILogger<QuizService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpStatusCode> CreateQuiz(QuizViewModel quizViewModel)
        {
            try
            {
                var quizDto = new QuizDto
                {
                    Title = quizViewModel.Title,
                    Description = quizViewModel.Description,
                    DurationMinutes = quizViewModel.DurationMinutes,
                    CreatedAt = DateTime.UtcNow,
                };                

                var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                quizDto.CreatedBy = GetUserIdFromToken(token).GetValueOrDefault();
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}", quizDto);

                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode;
                }

                _logger.LogWarning("Create Quiz failed with : {StatusCode}", response.StatusCode);
                return HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during create quiz.");
                return HttpStatusCode.BadRequest;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
