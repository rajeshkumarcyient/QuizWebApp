using Microsoft.AspNetCore.Mvc;
using QuizAppWeb.Models;
using QuizAppWeb.Service;

namespace QuizAppWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult ForgotPassword() => View();
        public IActionResult ResetPassword() => View();
        

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            bool isSuccess = await _userService.LoginAsync(loginModel);
            if (isSuccess)
            {
                //Get the token from API and put here.
                var token = "sample-jwt-token";

                return Json(new { success = true, token });
            }

            ModelState.AddModelError("", "Invalid login credentials.");
            return BadRequest(new { success = false, message = "Invalid login credentials." });
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
