using Eccomerce_Web.Modules.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eccomerce_Web.Modules.Admin.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Get-All-Succsessfull-Orders")]
        public async Task<IActionResult> GetAllSecsessfullOrders()
        {
            var result = await _adminService.GetAllSuccessfulOrders();
            return StatusCode(result.Status, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _adminService.GetAllUsers();
            return StatusCode(result.Status, result);
        }
    }
}
