using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
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
            public bool IsDescending { get; set; } = false;
            public bool SortOrder { get; internal set; }
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

        // Phân trang và tìm kiếm danh mục
        public async Task<PaginationCategoryResponse> GetCategoryWithPagination(CategorySearchRequest request)
        {
            try
            {
                var query = _context.Category.AsQueryable();

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(c => c.category_name.Contains(request.Keyword));
                }

                query = request.SortBy?.ToLower() switch
                {
                    "id" => request.IsDescending
                        ? query.OrderByDescending(c => c.category_id)
                        : query.OrderBy(c => c.category_id),
                    _ => request.IsDescending
                        ? query.OrderByDescending(c => c.category_name)
                        : query.OrderBy(c => c.category_name)
                };

                var totalCount = await query.CountAsync();

                var categories = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.category_id,
                        category_name = c.category_name,
                        description = c.description,
                        is_active = c.is_active,
                        created_at = c.created_at,
                        updated_at = c.updated_at
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
                _logger.LogError(ex, "Lỗi khi lấy danh mục phân trang");
                throw;
            }
        }

        // Lấy chi tiết danh mục theo id
        public async Task<CategoryResponseDto?> Detail(int id)
        {
            try
            {
                var category = await _context.Category
                    .Where(c => c.category_id == id)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.category_id,
                        category_name = c.category_name,
                        description = c.description,
                        is_active = c.is_active,
                        created_at = c.created_at,
                        updated_at = c.updated_at
                    })
                    .FirstOrDefaultAsync();

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết danh mục ID: {Id}", id);
                throw;
            }
        }

        // Lấy toàn bộ danh mục đang hoạt động
        public async Task<List<CategoryResponseDto>> GetAllCategory()
        {
            try
            {
                return await _context.Category
                    .Where(c => c.is_active)
                    .Select(c => new CategoryResponseDto
                    {
                        category_id = c.category_id,
                        category_name = c.category_name,
                        description = c.description,
                        created_at = c.created_at,
                        updated_at = c.updated_at,
                        is_active = c.is_active
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ danh mục");
                throw;
            }
        }

        // Tạo mới danh mục
        public async Task<CategoryResponseDto> CreateCategory(CategoryPostDto createDto)
        {
            try
            {
                var category = new Category
                {
                    category_name = createDto.category_name,
                    description = createDto.description,
                    is_active = createDto.is_active,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                _context.Category.Add(category);
                await _context.SaveChangesAsync();

                return new CategoryResponseDto
                {
                    category_id = category.category_id,
                    category_name = category.category_name,
                    description = category.description,
                    created_at = category.created_at,
                    updated_at = category.updated_at,
                    is_active = category.is_active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục mới");
                throw;
            }
        }

        // Xóa danh mục
        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Không tìm thấy danh mục để xóa với ID: {Id}", id);
                    return false;
                }

                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã xóa danh mục ID: {Id} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi xóa danh mục ID: {Id}", id);
                throw;
            }
        }

        // Sửa danh mục
        public async Task<CategoryResponseDto?> EditCategory(int id, CategoryPostDto categoryDto)
        {
            try
            {
                var category = await _context.Category.FindAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Không tìm thấy danh mục để cập nhật với ID: {Id}", id);
                    return null;
                }

                category.category_name = categoryDto.category_name;
                category.description = categoryDto.description;
                category.is_active = categoryDto.is_active;
                category.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Đã cập nhật danh mục ID: {Id} thành công", id);

                return new CategoryResponseDto
                {
                    category_id = category.category_id,
                    category_name = category.category_name,
                    description = category.description,
                    created_at = category.created_at,
                    updated_at = category.updated_at,
                    is_active = category.is_active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi cập nhật danh mục ID: {Id}", id);
                throw;
            }
        }

        internal async Task CreateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}