using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.Data;
using Eccomerce_Web.Models.User;
using Eccomerce_Web.Modules.User.Dtos.Response;
using Eccomerce_Web.Modules.User.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.User.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;
        private readonly IJWTService _jwtService;
        private readonly IPhoneSender _phoneSender;

        public UserService(DataContext db, IJWTService jwtService, IPhoneSender phoneSender)
        {
            _db = db;
            _jwtService = jwtService;
            _phoneSender = phoneSender;
        }

        public async Task<ApiResponse<OnlyUserInfoDto>> GetCurrentUserProfile(int userId)
        {
            var user = await _db.UserProfiles
                .Include(u => u.CartItems).ThenInclude(c => c.Product)
                .Include(o => o.Order).ThenInclude(p => p.Products)
                .Include(f => f.FavoritedProducts)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return ApiResponse<OnlyUserInfoDto>.NotFound("User not found");

            var userProfileDto = new OnlyUserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                isVerified = user.isVerified
            };

            return new ApiResponse<OnlyUserInfoDto>
            {
                Data = userProfileDto,
                Status = 200,
                Message = ""
            };
        }

        public async Task<ApiResponse<bool>> AddPhone(int userId, string phone)
        {
            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            var existing = await _db.UserProfiles.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            if (existing != null)
                return ApiResponse<bool>.BadRequest("This phone is already in use");

            await _phoneSender.SendPhoneMessage(phone);

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Verification code sent to your phone"
            };
        }

        public async Task<ApiResponse<bool>> VerifyPhone(int userId, string phone, string code)
        {
            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return ApiResponse<bool>.Unauthorized("User not found");

            var isVerified = await _phoneSender.VerifyPhoneCode(phone, code);
            if (!isVerified)
                return ApiResponse<bool>.BadRequest("Invalid verification code. Phone not saved.");

            user.PhoneNumber = phone;
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Phone verified and saved successfully"
            };
        }

        public async Task<ApiResponse<UserProfile>> DeleteCurrentUser(int userId)
        {
            var user = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return ApiResponse<UserProfile>.NotFound("Profile user not found");

            _db.UserProfiles.Remove(user);
            await _db.SaveChangesAsync();

            return new ApiResponse<UserProfile>
            {
                Data = user,
                Status = 200,
                Message = "Removed successfully"
            };
        }
    }
}