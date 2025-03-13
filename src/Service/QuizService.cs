using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
                    CreatedAt = DateTime.UtcNow
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

        public async Task<List<Quiz>> GetAllQuizzesAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{BaseUrl}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                return JsonConvert.DeserializeObject<List<Quiz>>(json, settings);
            }

            return [];
        }

        public async Task<QuizViewModel> GetQuizById(int quizId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{BaseUrl}/{quizId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var quiz = JsonConvert.DeserializeObject<QuizViewModel>(content);
                    return quiz;
                }

                _logger.LogWarning("Get Quiz failed with: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving quiz.");
                return null;
            }
        }

        public async Task<HttpStatusCode> UpdateQuiz(QuizViewModel quizViewModel)
        {
            try
            {
                var quizDto = new QuizDto
                {
                    QuizId = quizViewModel.QuizId,
                    Title = quizViewModel.Title,
                    Description = quizViewModel.Description,
                    DurationMinutes = quizViewModel.DurationMinutes,
                    CreatedAt = quizViewModel.CreatedAt,
                    CreatedBy = quizViewModel.CreatedBy
                };

                var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{quizDto.QuizId}", quizDto);

                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode;
                }

                _logger.LogWarning("Update Quiz failed with: {StatusCode}", response.StatusCode);
                return HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating quiz.");
                return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> DeleteQuiz(int quizId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{quizId}");

                if (response.IsSuccessStatusCode)
                {
                    return response.StatusCode;
                }

                _logger.LogWarning("Delete Quiz failed with: {StatusCode}", response.StatusCode);
                return HttpStatusCode.BadRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting quiz.");
                return HttpStatusCode.BadRequest;
            }
        }


        private int? GetUserIdFromToken(string token)
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
