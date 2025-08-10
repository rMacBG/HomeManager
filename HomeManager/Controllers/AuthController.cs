using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using HomeManager.Services.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeManager.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("register")]
        public IActionResult Register() => View();

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                foreach (var error in result.Errors!)
                {
                    ModelState.AddModelError(string.Empty, error);

                }
                return View(dto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet("login")]
        public IActionResult Login() => View();

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _authService.LoginAsync(username, password);

            if (!result.Success)
            {
                foreach (var error in result.Errors!)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                    return View();
                }
            }
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()!),
        new Claim(ClaimTypes.Name, result.Username!),
        new Claim(ClaimTypes.Role, result.Role!)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            Response.Cookies.Append("jwt", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(2)
            });

            return RedirectToAction("Index", "Home");
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
            //return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _authService.FindUserByEmailAsync(email); 
            if (user != null)
            {
                var token = Guid.NewGuid().ToString(); 
                await _authService.SavePasswordResetTokenAsync(user.Id, token); 

                var resetLink = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);
                await _authService.SendPasswordResetEmailAsync(email, resetLink); 
            }

            ViewBag.Message = "A reset link has been sent to your e-mail address.";
            return View();
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string token)
        {
            return View(model: token);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            var userId = await _authService.ValidatePasswordResetTokenAsync(token); 
            if (userId != null)
            {
                await _authService.UpdatePasswordAsync(userId.Value, newPassword); 
                ViewBag.Message = "Your password has been reset. You can now log in.";
            }
            else
            {
                ViewBag.Message = "Invalid or expired reset link.";
            }
            return View();
        }
    }
}
