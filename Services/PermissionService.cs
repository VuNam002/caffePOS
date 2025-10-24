using CaffePOS.Data;
using Microsoft.EntityFrameworkCore;
using CaffePOS.Model.DTOs.Requests;

namespace CaffePOS.Services
{
    public class PermissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AppDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PermissionResponseDto>> GetAllPermission(string? module = null)
        {
            try
            {
                var query = _context.Permissions.AsQueryable();

                // Filter theo module nếu có
                if (!string.IsNullOrWhiteSpace(module))
                {
                    query = query.Where(p => p.Module == module);
                }

                return await query
                    .Select(p => new PermissionResponseDto
                    {
                        permission_id = p.PermissionId,
                        permission_name = p.PermissionName,
                        description = p.Description,
                        module = p.Module,
                        create_at = p.CreatedAt
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay cac quyen.");
                throw;
            }
        }
    }
}