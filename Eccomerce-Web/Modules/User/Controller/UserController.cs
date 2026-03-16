using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Modules.User.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eccomerce_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [Authorize(Roles = "User, Admin")]
        [HttpGet("Get-Current-User")]
        public async Task<IActionResult> GetUserProfile()
        {
            var res = await _userService.GetCurrentUserProfile(GetUserId());
            return StatusCode(res.Status, res);
        }

        [Authorize]
        [HttpPost("add-phone")]
        public async Task<IActionResult> AddPhone(string phone)
        {
            var res = await _userService.AddPhone(GetUserId(), phone);
            return StatusCode(res.Status, res);
        }

        [Authorize]
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone(string phone, string code)
        {
            var res = await _userService.VerifyPhone(GetUserId(), phone, code);
            return StatusCode(res.Status, res);
        }

        [Authorize(Roles = "User, Admin")]
        [HttpDelete("Delete-Current-User")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var res = await _userService.DeleteCurrentUser(GetUserId());
            return StatusCode(res.Status, res);
        }
    }
}