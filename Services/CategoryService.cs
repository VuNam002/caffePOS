using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(AppDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // DTO cho request phân trang và tìm kiếm
        public class CategorySearchRequest
        {
            public string? Keyword { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public string? SortBy { get; set; } = "category_name";
            public bool SortOrder { get; set; } = false; // false = ASC, true = DESC
        }

        // DTO cho response phân trang
        public class PaginationCategoryResponse
        {
            public List<CategoryResponseDto> Categories { get; set; } = new List<CategoryResponseDto>();
            public int Page { get; set; }
            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
            public bool HasPrevious => Page > 1;
            public bool HasNext => Page < TotalPages;
        }

        public async Task<PaginationCategoryResponse> GetCategoryWithPagination(CategorySearchRequest request)
        {
            try
            {
                var query = _context.Category.AsQueryable();

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(c => c.CategoryName.Contains(request.Keyword));
                }

                query = request.SortBy?.ToLower() switch
                {
                    "category_name" => request.SortOrder
                        ? query.OrderByDescending(c => c.CategoryName)
                        : query.OrderBy(c => c.CategoryName),
                    "id" => request.SortOrder
                        ? query.OrderByDescending(c => c.CategoryId)
                        : query.OrderBy(c => c.CategoryId),
                    _ => request.SortOrder
                        ? query.OrderByDescending(c => c.CategoryName)
                        : query.OrderBy(c => c.CategoryName)
                };


                var totalCount = await query.CountAsync();

                var categories = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.CategoryId,
                        category_name = c.CategoryName,
                        description = c.Description,
                        // Thêm các properties khác cần mapping
                    })
                    .ToListAsync();

                return new PaginationCategoryResponse
                {
                    Categories = categories,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lối khi lấy sản phẩm phân trang");
                throw;
            }
        }

        // Thêm method để lấy category theo ID (tuỳ chọn)
        public async Task<CategoryResponseDto?> Detail(int id)
        {
            try
            {
                var category = await _context.Category
                    .Where(c => c.CategoryId == id)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.CategoryId,
                        category_name = c.CategoryName,
                        description = c.Description,
                        // Thêm các properties khác cần mapping
                    })
                    .FirstOrDefaultAsync();

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh mục sản phẩm theo ID: {Id}", id);
                throw;
            }
        }

        //Lấy toàn bộ danh sách category
        public async Task<List<CategoryResponseDto>> GetAllCategory()
        {
            try
            {
                return await _context.Category
                    .Where(c => c.IsActive)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.CategoryId,
                        category_name = c.CategoryName,
                        description = c.Description,
                        created_at = c.CreatedAt,
                        updated_at = c.UpdatedAt,
                        is_active = c.IsActive
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh mục sản phẩm");
                throw;
            }
        }
        //Thêm mới category

        public async Task<CategoryResponseDto> CreateCategory(Category category)
        {
            try
            {
                if (category == null)
                    throw new ArgumentNullException(nameof(category));

                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;

                _context.Category.Add(category);
                await _context.SaveChangesAsync();

                return new CategoryResponseDto
                {
                    category_id = category.CategoryId,
                    category_name = category.CategoryName,
                    description = category.Description,
                    created_at = category.CreatedAt,
                    updated_at = category.UpdatedAt,
                    is_active = category.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục mới");
                throw;
            }
        }

        //Xóa danh mục sản phẩm
        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Category.FirstOrDefaultAsync(p => p.CategoryId == id);
                if (category == null)
                {
                    _logger.LogWarning("Không tìm thấy sản phẩm");
                    return false;
                }
                _context.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã xóa sản phẩm thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi xóa sản phẩm");
                return false;
            }
        }
        //Sửa danh mục sản phẩm
        public async Task<CategoryResponseDto?> EditCategory(int id, CategoryPostDto categoryDto)
        {
            try
            {
                // Tìm category theo ID
                var category = await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                {
                    _logger.LogWarning("Không tìm thấy danh mục với ID: {Id}", id);
                    return null;
                }

                // Cập nhật thông tin
                category.CategoryName = categoryDto.category_name;
                category.Description = categoryDto.description;
                category.UpdatedAt = DateTime.Now;

                // Sửa lại phần này - bỏ HasValue
                category.IsActive = categoryDto.is_active;

                _context.Category.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Đã cập nhật danh mục ID: {Id} thành công", id);

                // Trả về response DTO
                return new CategoryResponseDto
                {
                    category_id = category.CategoryId,
                    category_name = category.CategoryName,
                    description = category.Description,
                    created_at = category.CreatedAt,
                    updated_at = category.UpdatedAt,
                    is_active = category.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi cập nhật danh mục ID: {Id}", id);
                throw;
            }
        }
    }
}