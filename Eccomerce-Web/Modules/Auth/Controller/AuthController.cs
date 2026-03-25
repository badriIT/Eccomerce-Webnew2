using System.ComponentModel.DataAnnotations;
using Eccomerce_Web.Modules.Auth.Dtos.Request;
using Eccomerce_Web.Modules.Auth.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Eccomerce_Web.Modules.Auth.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [EnableRateLimiting("verify")]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(string code, string email)
        {
            var res = await _authService.VerifyEmail(code, email);
            return StatusCode(res.Status, res);
        }


        [EnableRateLimiting("verify")]
        [HttpPost("Send-Verification-Code")]
        public async Task<IActionResult> SendEmailVerificationCode([Required][EmailAddress] string email)
        {
            var res = await _authService.SendEmailVerificationCode(email);
            return StatusCode(res.Status, res);
        }


        [EnableRateLimiting("register")]
        [HttpPost("Register")]
        public async Task<IActionResult> AddUser(RegisterDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _authService.Register(user);
            return StatusCode(res.Status, res);
        }

        [EnableRateLimiting("login")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var res = await _authService.Login(login);
            return StatusCode(res.Status, res);
        }
    }
}
