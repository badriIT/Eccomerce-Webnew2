using System.ComponentModel.DataAnnotations;
using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.Common.Services.ServiceModels;
using Eccomerce_Web.Data;
using Eccomerce_Web.Models;
using Eccomerce_Web.Models.User;
using Eccomerce_Web.Modules.Auth.Dtos.Request;
using Eccomerce_Web.Modules.Auth.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eccomerce_Web.Modules.Auth.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _db;
        private readonly IJWTService _jwtService;
        private readonly IEmailSender _emailSender;
       

        public AuthService(DataContext db, IJWTService jwtService, IEmailSender emailSender)
        {
            _db = db;
            _jwtService = jwtService;
            _emailSender = emailSender;
          
        }

        public async Task<ApiResponse<bool>> VerifyEmail(string code, string email)
        {
            var userProfile = await _db.UserProfiles
                .FirstOrDefaultAsync(x => x.Email == email);

           




            if (userProfile == null)
                return ApiResponse<bool>.BadRequest("User Not Found");

            
            var CodeTime = userProfile.CodeCreatedAt.Value;

          

            if ((DateTime.UtcNow - CodeTime).TotalMinutes > 10)
            {
                return ApiResponse<bool>.BadRequest("Code Expired Try again");
            }


            if (userProfile.VerificationCode != code)
            {
                userProfile.VerificationAttempts += 1;

                if (userProfile.VerificationAttempts >= 10)
                {
                    userProfile.VerificationCode = null;
                    await _db.SaveChangesAsync();
                    return ApiResponse<bool>.BadRequest("too many attempts, code reset");
                }

                if (userProfile.VerificationAttempts >= 3)
                {
                    await _db.SaveChangesAsync();
                    return ApiResponse<bool>.BadRequest("too many attempts, try again later");
                }

                await _db.SaveChangesAsync();
                return ApiResponse<bool>.BadRequest("Code is not correct");
            }



            userProfile.isVerified = true;
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "User Verified Successfully! Now You Can Login"
            };
        }

        public async Task<ApiResponse<bool>> SendEmailVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email))
                return ApiResponse<bool>.BadRequest("Write your email");

            var foundUser = await _db.UserProfiles.FirstOrDefaultAsync(p => p.Email == email);

            if (foundUser == null)
                return ApiResponse<bool>.BadRequest("This email is not registered");

            string code = Guid.NewGuid().ToString("N")[..5].ToUpper();
            foundUser.VerificationCode = code;

            await _emailSender.SendEmailAsync(email, code);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Data = true,
                Status = 200,
                Message = "Email sent!"
            };
        }

        public async Task<ApiResponse<string>> Register(RegisterDto user)
        {
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
                return ApiResponse<string>.BadRequest("Email already exists");

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var newUser = new Models.User.User
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

            await SendEmailVerificationCode(user.Email);

            return ApiResponse<string>.Created("success", "Created Successfully");
        }

        public async Task<ApiResponse<object>> Login(LoginDto login)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.email == login.Email);

            var user = await _db.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null && admin == null)
                return ApiResponse<object>.NotFound("User not found");

            if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword))
            {
                var token = _jwtService.GetUserToken(user.UserProfile);
                return new ApiResponse<object>
                {
                    Data = token,
                    Status = 200,
                    Message = ""
                };
            }

            if (admin != null && BCrypt.Net.BCrypt.Verify(login.Password, admin.password))
            {
                var tokenAdmin = _jwtService.GetAdminToken(admin);
                return new ApiResponse<object>
                {
                    Data = tokenAdmin,
                    Status = 200,
                    Message = ""
                };
            }

            return ApiResponse<object>.BadRequest("Invalid Password");
        }
    }
}
