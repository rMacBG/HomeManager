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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
