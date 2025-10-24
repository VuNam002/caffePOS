using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    [Authorize] // Thêm Authorization Bearer token
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PermissionResponseDto>>> GetAllPermission(
            [FromQuery] string? module = null) 
        {
            try
            {
                var permissions = await _permissionService.GetAllPermission(module);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all permissions.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}