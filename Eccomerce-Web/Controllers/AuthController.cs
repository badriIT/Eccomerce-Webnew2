using Eccomerce_Web.CORE;
using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly IJWTService _JWTService;
        private readonly IEmailSender _emailSender;
        private readonly IPhoneSender _phoneSender;


        public AuthController(DataContext db, IJWTService jwt, IEmailSender emailSender, IPhoneSender phoneSender)
        {
            _db = db;
            _JWTService = jwt;
            _emailSender = emailSender;
            _phoneSender = phoneSender;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> VerifyEmail()
        {
            await _emailSender.SendEmailAsync(
                "badriberidze042@gmail.com"
            );
            return Ok("Email sent!");
        }

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSMSToPhone()
        {
            await _phoneSender.SendPhoneMessage(
                "555585103"
            );
            return Ok("SMS sent!");
        }

        


        [HttpPost("Register")]
        public async Task<IActionResult> AddUser(RegisterDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "ModelState is not Valid"
                });

            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
            {
                ApiResponse<bool> ResponseNotFound = new()
                {
                    Data = false,
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email already exists"

                };

                return BadRequest(ResponseNotFound);
            }


            var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var newUser = new User
            {
                UserName = user.FullName,
                Email = user.Email,
                HashedPassword = hashPassword
            };

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                FullName = user.FullName,
                Email = user.Email,
                UserId = newUser.Id
            };


            await _db.UserProfiles.AddAsync(userProfile);
            await _db.SaveChangesAsync();

            ApiResponse<string> ResponseOK = new()
            {
                Data = "success",
                Status = StatusCodes.Status201Created,
                Message = "Created Seccsessfully"
            };

            return Ok(ResponseOK);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.email == login.Email);

            var user = await _db.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null && admin == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found"
                });
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword))
            {
                var token = _JWTService.GetUserToken(user.UserProfile);
                return Ok(token);
            }

            if (admin != null && BCrypt.Net.BCrypt.Verify(login.Password, admin.password))
            {
                var tokenAdmin = _JWTService.GetAdminToken(admin);
                return Ok(tokenAdmin);
            }

            return BadRequest(new ApiResponse<bool>
            {
                Data = false,
                Status = StatusCodes.Status400BadRequest,
                Message = "Invalid Password"
            });
        }










    }
}
