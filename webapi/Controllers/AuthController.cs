using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [ApiController]
    [Route("authController")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin user)
        {
            var principal = await _authService.AuthenticateAsync(user.Username, user.Password);

            if (principal != null)
            {
                await HttpContext.SignInAsync("CookieAuth", principal);
                return Ok();
            }

            return Unauthorized();
        }
    }
}
