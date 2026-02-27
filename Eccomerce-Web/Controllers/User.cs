using Eccomerce_Web.Data;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Models;
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

        public UserController(DataContext db) => _db = db;





        [HttpPost("Register")]
        public async Task<IActionResult> AddUser(RegisterDto user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);


            User newUser = new()
            {
                Email = user.Email,
                HashedPassword = hashPassword
            };




           
            await _db.Users.AddAsync(newUser);
            UserProfile newUserProfile = new()
            {
                UserId = newUser.Id,
                Email = newUser.Email,
                User = newUser
            };
            await _db.UserProfiles.AddAsync(newUserProfile);
            await _db.SaveChangesAsync();
            return Ok("success");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (login == null)
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword))
            {
                return BadRequest("Invalid password.");
            }

            var UserProfile = await _db.UserProfiles.FirstOrDefaultAsync(up => up.UserId == user.Id);

            return Ok(UserProfile);
        }


        [HttpGet("Get-User")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _db.Users.ToListAsync();

            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            return Ok(user);
        }


        [HttpGet("Get-User-byid/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return BadRequest("Invalid user data.");

            return Ok("success");
        }


        [HttpDelete("Delete-User-byid/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            _db.Remove(user);
            await _db.SaveChangesAsync();
            return Ok("success");
        }



    }
}
