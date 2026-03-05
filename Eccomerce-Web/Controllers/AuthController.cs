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

        public AuthController(DataContext db, IJWTService jwt )
        {
            _db = db;
            _JWTService = jwt;
        }





        [HttpPost("Register")]
        public async Task<IActionResult> AddUser(RegisterDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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


            //var userDto = new UserDto
            //{
            //    Email = newUser.Email
            //};



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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found"
                });
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword))
                return BadRequest("Invalid password");

          
            var token = _JWTService.GetUserToken(user.UserProfile); 

           

            return Ok(new
            {
                Token = token,
               
            });
        }



        



    }
}
