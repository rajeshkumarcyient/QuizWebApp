using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAppWeb.Models;
using QuizAppWeb.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuizAppWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IQuizService _quizService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IUserService userService, IQuizService quizService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _quizService = quizService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Register() => View();
        public IActionResult ForgotPassword() => View();
        public IActionResult ResetPassword() => View();

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            bool isSuccess = await _userService.LoginAsync(loginModel);
    
            if (!isSuccess)
            {
                return BadRequest(new { success = false, message = "Invalid login credentials." });
            }

            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "Authentication token missing." });
            }

            var (userRole, userId) = GetUserDetailsFromToken(token);
    
            if (string.IsNullOrEmpty(userRole))
            {
                return BadRequest(new { success = false, message = "Invalid token or role missing." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            string redirectUrl = userRole == "Admin" ? "/Admin/Dashboard" : "/Home/Attempt";

            return Json(new { success = true, redirectUrl });
        }

        public (string role, string userId) GetUserDetailsFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return (null, null);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier);

            return (roleClaim?.Value, userIdClaim?.Value);
        }
        

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel userModel)
        {
            bool isSuccess = await _userService.RegisterUserAsync(userModel);
            if (isSuccess)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(userModel);
        }

        
        public IActionResult AccessDenied() {
            return View();
        }

    }
}
