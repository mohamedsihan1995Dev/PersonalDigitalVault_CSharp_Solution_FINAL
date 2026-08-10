//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc; 
//using PersonalDigitalVault.Api.DTOs.Admin; 
//using PersonalDigitalVault.Api.Interfaces.Services;
//namespace PersonalDigitalVault.Api.Controllers;
//[ApiController][Authorize(Roles="Administrator")][Route("api/admin")] public class AdminController(IAdminService service):ControllerBase
//{ [HttpGet("dashboard")] public async Task<IActionResult> Dashboard()=>Ok(await service.GetDashboardAsync()); [HttpGet("users")] public async Task<IActionResult> Users()=>Ok(await service.GetUsersAsync()); [HttpPut("users/{id:int}/status")] public async Task<IActionResult> Status(int id,UpdateUserStatusDto dto){await service.UpdateUserStatusAsync(id,dto);return NoContent();} }


//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace PersonalDigitalVault.Api.Controllers;

//[ApiController]
//[Route("api/admin")]

//// IMPORTANT:
//// JWT-la Role = Admin irukkura user mattum
//// indha controller access panna mudiyum.
//[Authorize(Roles = "Admin")]
//public class AdminController : ControllerBase
//{
//    // GET:
//    // /api/admin/dashboard
//    //
//    // Ippo role authorization verify panna simple response.
//    [HttpGet("dashboard")]
//    public IActionResult Dashboard()
//    {
//        return Ok(new
//        {
//            message = "Admin access successful."
//        });
//    }
//
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.Api.DTOs.Admin;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;

[ApiController]
[Route("api/admin")]

//
// IMPORTANT:
// Role = Admin JWT mattum
// indha controller access panna mudiyum.
//
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(
        IAdminService adminService)
    {
        _adminService = adminService;
    }


    // =========================================
    // GET /api/admin/dashboard
    // =========================================
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var result =
            await _adminService
                .GetDashboardAsync();

        return Ok(result);
    }


    // =========================================
    // GET /api/admin/users
    // =========================================
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result =
            await _adminService
                .GetUsersAsync();

        return Ok(result);
    }


    // =========================================
    // PUT /api/admin/users/{id}/status
    // =========================================
    [HttpPut("users/{id}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        int id,
        UpdateUserStatusDto dto)
    {
        try
        {
            await _adminService
                .UpdateUserStatusAsync(
                    id,
                    dto);

            return Ok(new
            {
                message =
                    dto.IsActive
                        ? "User enabled successfully."
                        : "User disabled successfully."
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}