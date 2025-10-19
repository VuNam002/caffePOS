using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CaffePOS.Services
{
    public class ItemsService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemsService> _logger;

        public ItemsService(AppDbContext context, ILogger<ItemsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // DTO cho request phân trang & tìm kiếm
        public class ItemSearchRequest
        {
            public string? Keyword { get; set; }
            public int? CategoryId { get; set; }
            public bool? IsActive { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 10;
            public string? SortBy { get; set; } = "name";
            public bool SortDesc { get; set; } = false;
        }

        // DTO cho response phân trang
        public class PaginatedItemsResponse
        {
            public List<ItemResponseDto> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public bool HasPrevious => Page > 1;
            public bool HasNext => Page < TotalPages;
        }

        public async Task<PaginatedItemsResponse> GetItemsWithPagination(ItemSearchRequest request)
        {
            try
            {
                var query = _context.Items
                    .Include(i => i.Category)
                    .AsQueryable();

                // Áp dụng tìm kiếm theo keyword
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var keyword = request.Keyword.ToLower();

                    // === SỬA LỖI Ở ĐÂY ===
                    // Thêm kiểm tra != null để tránh lỗi crash khi Name hoặc Description trống
                    query = query.Where(i =>
                        (i.Name != null && i.Name.ToLower().Contains(keyword)) ||
                        (i.Description != null && i.Description.ToLower().Contains(keyword)) ||
                        (i.Category != null && i.Category.CategoryName != null && i.Category.CategoryName.ToLower().Contains(keyword))
                    );
                }

                // Lọc theo category
                if (request.CategoryId.HasValue && request.CategoryId > 0)
                {
                    query = query.Where(i => i.CategoryId == request.CategoryId.Value);
                }

                // Lọc theo trạng thái active
                if (request.IsActive.HasValue)
                {
                    query = query.Where(i => i.IsActive == request.IsActive.Value);
                }

                // Sắp xếp
                query = request.SortBy?.ToLower() switch
                {
                    "price" => request.SortDesc ?
                        query.OrderByDescending(i => i.Price) :
                        query.OrderBy(i => i.Price),
                    "createdat" => request.SortDesc ?
                        query.OrderByDescending(i => i.CreatedAt) :
                        query.OrderBy(i => i.CreatedAt),
                    "name" => request.SortDesc ?
                        query.OrderByDescending(i => i.Name) :
                        query.OrderBy(i => i.Name),
                    _ => request.SortDesc ?
                        query.OrderByDescending(i => i.Name) :
                        query.OrderBy(i => i.Name)
                };

                // Đếm tổng số record (trước khi phân trang)
                var totalCount = await query.CountAsync();

                // Áp dụng phân trang
                var items = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(i => new ItemResponseDto
                    {
                        item_id = i.ItemId,
                        name = i.Name,
                        description = i.Description,
                        price = i.Price,
                        category_id = i.CategoryId,
                        category_name = i.Category == null ? null : (string)i.Category.CategoryName,
                        image_url = i.ImageUrl,
                        is_active = i.IsActive,
                        created_at = i.CreatedAt,
                        updated_at = i.UpdatedAt
                    })
                    .ToListAsync();

                return new PaginatedItemsResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy sản phẩm với phân trang");
                throw;
            }
        }

        // Giữ lại phương thức cũ để backward compatibility
        public async Task<List<ItemResponseDto>> GetAllItems()
        {
            try
            {
                return await _context.Items
                    .Include(i => i.Category)
                    .Where(i => i.IsActive) // Chỉ lấy active items
                    .Select(i => new ItemResponseDto
                    {
                        item_id = i.ItemId,
                        name = i.Name,
                        description = i.Description,
                        price = i.Price,
                        category_id = i.CategoryId,
                        category_name = i.Category == null ? null : (string)i.Category.CategoryName,
                        image_url = i.ImageUrl,
                        is_active = i.IsActive,
                        created_at = i.CreatedAt,
                        updated_at = i.UpdatedAt
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy toàn bộ sản phẩm");
                throw;
            }
        }

        // Lấy chi tiết sản phẩm theo id
        public async Task<ItemResponseDto?> Detail(int id)
        {
            try
            {
                var item = await _context.Items
                    .Include(i => i.Category)
                    .Where(i => i.ItemId == id)
                    .Select(i => new ItemResponseDto
                    {
                        item_id = i.ItemId,
                        name = i.Name,
                        description = i.Description,
                        price = i.Price,
                        category_id = i.CategoryId,
                        category_name = i.Category == null ? null : (string)i.Category.CategoryName,
                        image_url = i.ImageUrl,
                        is_active = i.IsActive,
                        created_at = i.CreatedAt,
                        updated_at = i.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết sản phẩm id {id}");
                throw;
            }
        }

        //Thêm sản phẩm mới
        public async Task<ItemResponseDto> CreateItem(Items item)
        {
            try
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                // Kiểm tra tồn tại Category
                if (item.CategoryId > 0)
                {
                    var categoryExists = await _context.Category
                        .AnyAsync(c => c.CategoryId == item.CategoryId);

                    if (!categoryExists)
                        throw new ArgumentException($"Category với id {item.CategoryId} không tồn tại");
                }

                item.CreatedAt = DateTime.Now;
                item.UpdatedAt = DateTime.Now;

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                // Lấy lại thông tin đầy đủ (bao gồm cả Category) để trả về
                var createdItem = await _context.Items
                    .Include(i => i.Category)
                    .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

                // Nếu createdItem là null (không thể xảy ra) thì xử lý
                if (createdItem == null)
                {
                    _logger.LogError("Không thể tìm thấy item vừa tạo: {ItemId}", item.ItemId);
                    // Trả về từ item gốc, dù không có category name
                    return new ItemResponseDto { item_id = item.ItemId, name = item.Name, /*...*/ };
                }

                return new ItemResponseDto
                {
                    item_id = createdItem.ItemId,
                    name = createdItem.Name,
                    description = createdItem.Description,
                    price = createdItem.Price,
                    category_id = createdItem.CategoryId,
                    category_name = (string)createdItem.Category?.CategoryName,
                    image_url = createdItem.ImageUrl,
                    is_active = createdItem.IsActive,
                    created_at = createdItem.CreatedAt,
                    updated_at = createdItem.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm mới");
                throw;
            }
        }

        //Sửa sản phẩm
        public async Task<ItemResponseDto?> EditItem(int id, ItemsPostDto itemDto)
        {
            try
            {
                var existingItem = await _context.Items
                    .Include(i => i.Category) 
                    .FirstOrDefaultAsync(i => i.ItemId == id);

                if (existingItem == null)
                {
                    _logger.LogWarning("Không tìm thấy sản phẩm có id = {Id}", id);
                    return null;
                }
                if (!string.IsNullOrEmpty(itemDto.name))
                    existingItem.Name = itemDto.name;

                if (itemDto.description != null) 
                    existingItem.Description = itemDto.description;

                if (itemDto.price.HasValue)
                    existingItem.Price = itemDto.price.Value;

                if (itemDto.category_id > 0)
                {
                    var category = await _context.Category.FindAsync(itemDto.category_id);
                    if (category != null)
                    {
                        existingItem.CategoryId = itemDto.category_id;
                        existingItem.Category = category; 
                    }
                    else
                    {
                        _logger.LogWarning("Không tìm thấy Category ID {CatId} khi cập nhật Item {ItemId}", itemDto.category_id, id);
                    }
                }

                if (itemDto.image_url != null)
                    existingItem.ImageUrl = itemDto.image_url;

                existingItem.IsActive = itemDto.is_active;
                existingItem.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ItemResponseDto
                {
                    item_id = existingItem.ItemId,
                    name = existingItem.Name,
                    description = existingItem.Description,
                    price = existingItem.Price,
                    category_id = existingItem.CategoryId,
                    category_name = (string)existingItem.Category?.CategoryName,
                    image_url = existingItem.ImageUrl,
                    is_active = existingItem.IsActive,
                    created_at = existingItem.CreatedAt,
                    updated_at = existingItem.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm id = {Id}", id);
                throw;
            }
        }

        //Xóa sản phẩm
        public async Task<bool> DeleteItem(int id)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(p => p.ItemId == id);
                if (item == null)
                {
                    _logger.LogWarning("Không tìm thấy sản phẩm ID {Id} để xóa", id);
                    return false;
                }
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Đã xóa sản phẩm ID {Id} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm ID {Id}", id);
                throw;
            }
        }
    }
}