using System.IdentityModel.Tokens.Jwt;
using Eccomerce_Web.Data;
using Eccomerce_Web.Models;
using Eccomerce_Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Eccomerce_Web.Dtos;
using System.Xml;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {




        private readonly DataContext _db ;
        private readonly IJWTService _IJWTService;

        public UserController(DataContext db, IJWTService JW)
        {
            _db = db;
            _IJWTService = JW;

        }



        //[HttpGet("Get-User")]
        //public async Task<IActionResult> GetUser()
        //{
        //    var user = await _db.Users.Include(s => s.UserProfile).ToListAsync();

        //    // if (user == null)
        //    // {
        //    //     return BadRequest("Invalid user."); why? just return blank array 
        //    // }

        //    return Ok(user);
        //}


        //////////////////////////////// stop here uppen cuz it is admin functionality and we will do it in admin controller


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("Get-Current-User")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var UserRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null)
                return Unauthorized();

            if (UserRoleClaim == null)
                return Unauthorized();



            int userId = int.Parse(userIdClaim.Value);

            var user = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId);

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

            ApiResponse<UserProfile> Res = new()
            {
                Data = user,
                Status = StatusCodes.Status200OK,
                Message = ""
            };

            return Ok(user);
        }


        /// this is update user badrii !!!!!!!!!!
        /// 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpPut("Update-Current-User")]
        public async Task<IActionResult> UpdateCurrentUser(UserProfileUPTDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Convert.ToInt32(userIdClaim.Value);

            var profileUser = await _db.UserProfiles.Include(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (profileUser == null)
                return NotFound("Profile user not found");

            profileUser.FullName = dto.FullName;




            var ExistingPhoneNumlUser = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber && u.UserId != userId);

            if (ExistingPhoneNumlUser != null)
            {
                ApiResponse<bool> ResEmailConflict = new()
                {
                    Data = false,
                    Status = StatusCodes.Status409Conflict,
                    Message = "Phone is already in use by another user"
                };
                return Conflict(ResEmailConflict);
            }



            profileUser.PhoneNumber = dto.PhoneNumber;

            var ExistingEmailUser = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.UserId != userId);

            if (ExistingEmailUser != null)
            {
                ApiResponse<bool> ResEmailConflict = new()
                {
                    Data = false,
                    Status = StatusCodes.Status409Conflict,
                    Message = "Email is already in use by another user"
                };
                return Conflict(ResEmailConflict);
            } 

                profileUser.Email = dto.Email;

            await _db.SaveChangesAsync();

            ApiResponse<UserProfile> res = new()
            {
                Data = profileUser,
                Status = StatusCodes.Status200OK,
                Message = "Updated successfully"
            };


            return Ok(res);
        }


        /// this is delete badri!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpDelete("Delete-Current-User")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int userId = Convert.ToInt32(userIdClaim.Value);

            var profileUser = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (profileUser == null)
                return NotFound("Profile user not found");

            _db.Remove(profileUser);
            await _db.SaveChangesAsync();

            ApiResponse<UserProfile> res = new()
            {
                Data = profileUser,
                Status = StatusCodes.Status200OK,
                Message = "Removed successfully"
            };

            return Ok(res);
        }



        //[HttpDelete("Delete-User-byid/{id}")]
        //public async Task<IActionResult> GetUserById(int id)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        //    if (user == null)
        //    {

        //        ApiResponse<bool> ResNotFound = new()
        //        {
        //            Data = false,
        //            Status = StatusCodes.Status404NotFound,
        //            Message = "User not found"

        //        };


        //        return NotFound(ResNotFound);
        //    }

        //    _db.Remove(user);
        //    await _db.SaveChangesAsync();
        //    return Ok("success");
        //}


        //////////////////////////////// stop here uppen cuz it is admin functionality and we will do it in admin controller
        ///



        /// if you want to make any changes to users acc like now it is good for you tu build user profile update you know with new dto maybe btw for finding use claims like jwt stores this info in it.
        /// now the user can only get his profile but if you want to make any changes to users acc like now it is good for you tu build user profile update you know with new dto maybe btw for finding use claims like jwt stores this info in it.
        /// also now works jwt auth in user login register and getting look throught it.
        /// role based access is on also
        /// for now make update.
        /// everything is good now KEEP GRWING!!!
    }
}
