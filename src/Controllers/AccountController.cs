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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult ForgotPassword() => View();
        public IActionResult ResetPassword() => View();
        

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            bool isSuccess = await _userService.LoginAsync(loginModel);
            if (isSuccess)
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        
                string userRole = GetUserRoleFromToken(token); 
                string redirectUrl = userRole == "Admin" ? "/Admin/Dashboard" : "/Home/Attempt";
        
                return Json(new { success = true, redirectUrl });
            }

            return BadRequest(new { success = false, message = "Invalid login credentials." });
        }

        public string GetUserRoleFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role);

            return roleClaim?.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]UserModel userModel)
        {
            bool isSuccess = await _userService.RegisterUserAsync(userModel);
            if (isSuccess)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(userModel);
        }
    }
}
