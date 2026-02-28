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
    public class UserController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly IJWTService _JWTService;

        public UserController(DataContext db, IJWTService jwt )
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
                Email = user.Email,
                HashedPassword = hashPassword
            };

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            //var userDto = new UserDto
            //{
            //    Email = newUser.Email
            //};



            ApiResponse<string> ResponseOK = new()
            {
                Data = "success",
                Status = StatusCodes.Status200OK,
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

            // ✅ Generate token from database user
            var token = _JWTService.GetUserToken(user.UserProfile); // err here

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.UserProfile?.FullName,
                PhoneNumber = user.UserProfile?.PhoneNumber
            };

            return Ok(new
            {
                Token = token,
                User = userDto
            });
        }



        [HttpGet("Get-User")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _db.Users.ToListAsync();

            // if (user == null)
            // {
            //     return BadRequest("Invalid user."); why? just return blank array 
            // }

            return Ok(user);
        }


        [HttpGet("Get-User-byid/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {


                ApiResponse<bool> ResNotFounds = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found"

                };





                return NotFound(ResNotFounds);
            }




            ApiResponse<UserProfile> ResNotFound = new()
            {
                Data = user,  
                Status = StatusCodes.Status404NotFound,
                Message = "User not found"
            };


            return Ok("success");
        }


        [HttpDelete("Delete-User-byid/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {

                ApiResponse<bool> ResNotFound = new()
                {
                    Data = false,
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found"

                };


                return NotFound(ResNotFound);
            }

            _db.Remove(user);
            await _db.SaveChangesAsync();
            return Ok("success");
        }



    }
}
