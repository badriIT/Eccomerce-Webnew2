//using System.Security.Claims;
//using Eccomerce_Web.Common.Dtos.Responses;
//using Eccomerce_Web.Common.Services.Interfaces;
//using Eccomerce_Web.Data;
//using Eccomerce_Web.Dtos;
//using Eccomerce_Web.Models.User;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Eccomerce_Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly DataContext _db ;
//        private readonly IJWTService _IJWTService;
//        private readonly IPhoneSender _phoneSender;

//        public UserController(DataContext db, IJWTService JW, IPhoneSender ph )
//        {
//            _db = db;
//            _IJWTService = JW;
//            _phoneSender = ph;

//        }


//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
//        [HttpGet("Get-Current-User")]
//        public async Task<IActionResult> GetUserProfile()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

//            if (userIdClaim == null || userRoleClaim == null)
//                return Unauthorized(new ApiResponse<bool>
//                {
//                    Data = false,
//                    Status = StatusCodes.Status401Unauthorized,
//                    Message = "user Id Claim and user Role Claim is null"
//                });

//            int userId = int.Parse(userIdClaim.Value);

//            var user = await _db.UserProfiles
//                .Include(u => u.CartItems)
//                    .ThenInclude(c => c.Product)
//                    .Include(o => o.Order).ThenInclude(p => p.Products)
//                     .Include(f => f.FavoritedProducts) .AsNoTracking()
//                .FirstOrDefaultAsync(u => u.UserId == userId);

//            if (user == null)
//                return Unauthorized(new ApiResponse<bool>
//                {
//                    Data = false,
//                    Status = StatusCodes.Status404NotFound,
//                    Message = "User not found"
//                });


//            OnlyUserInfoDto userProfileDto = new OnlyUserInfoDto
//            {
//                Id = user.Id,
//                Email = user.Email,
//                FullName = user.FullName,
//                PhoneNumber = user.PhoneNumber,
//                Role = user.Role,
//                isVerified = user.isVerified,
//            };



//            return Ok(new ApiResponse<OnlyUserInfoDto>
//            {
//                Data = userProfileDto, 
//                Status = StatusCodes.Status200OK,
//                Message = "" /////// display: flex; 
//            });
//        }


       
        


//        [HttpPost("add-phone")]
//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//        public async Task<IActionResult> AddPhone(string phone)
//        {
//            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var user = await _db.UserProfiles.FirstOrDefaultAsync(o => o.UserId == userId);
//            var FoundeUserWithPhone = await _db.UserProfiles.FirstOrDefaultAsync(p => p.PhoneNumber == phone);
//            if (user == null)
//                return Unauthorized(new ApiResponse<bool> { Data = false, Status = StatusCodes.Status401Unauthorized, Message = "User not found" });
//            if(FoundeUserWithPhone != null)
//            {
//                return BadRequest(new ApiResponse<bool> { Data = false, Status = StatusCodes.Status400BadRequest, Message = "this phone is already in use" });
//            }
            

//            await _phoneSender.SendPhoneMessage(phone);

//            return Ok(new ApiResponse<bool>
//            {
//                Data = true,
//                Status = StatusCodes.Status200OK,
//                Message = "Verification code sent to your phone"
//            });
//        }

//        [HttpPost("verify-phone")]
//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//        public async Task<IActionResult> VerifyPhone(string phone, string code)
//        {
//            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var user = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
//            if (user == null)
//                return Unauthorized(new ApiResponse<bool> { Data = false, Status = StatusCodes.Status401Unauthorized, Message = "User not found" });

            
//            var isVerified = await _phoneSender.VerifyPhoneCode(phone, code);

//            if (!isVerified)
//            {
               
//                return BadRequest(new ApiResponse<bool>
//                {
//                    Data = false,
//                    Status = StatusCodes.Status400BadRequest,
//                    Message = "Invalid verification code. Phone not saved."
//                });
//            }

         
//            user.PhoneNumber = phone;
          
//            await _db.SaveChangesAsync();

//            return Ok(new ApiResponse<bool>
//            {
//                Data = true,
//                Status = StatusCodes.Status200OK,
//                Message = "Phone verified and saved successfully"
//            });
//        }


//        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
//        [HttpDelete("Delete-Current-User")]
//        public async Task<IActionResult> DeleteCurrentUser()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

//            if (userIdClaim == null)
//                return Unauthorized(new ApiResponse<bool>
//                {
//                    Data = false,
//                    Status = StatusCodes.Status401Unauthorized,
//                    Message = "user Id Claim is null"
//                });

//            int userId = Convert.ToInt32(userIdClaim.Value);

//            var profileUser = await _db.UserProfiles
//                .FirstOrDefaultAsync(u => u.UserId == userId);

//            if (profileUser == null)
//                return NotFound(new ApiResponse<bool>
//                {
//                    Data = false,
//                    Status = StatusCodes.Status404NotFound,
//                    Message = "Profile user not found"
//                });

//            _db.Remove(profileUser);
//            await _db.SaveChangesAsync();

//            ApiResponse<UserProfile> res = new()
//            {
//                Data = profileUser,
//                Status = StatusCodes.Status200OK,
//                Message = "Removed successfully"
//            };

//            return Ok(res);
//        }
       


//    }
//}
