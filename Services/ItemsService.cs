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

        // Lấy toàn bộ sản phẩm
        public async Task<List<ItemResponseDto>> GetAllItems()
        {
            try
            {
                return await _context.Items
                    .Include(i => i.Category)
                    .Select(i => new ItemResponseDto
                    {
                        item_id = i.ItemId,
                        name = i.Name,
                        description = i.Description,
                        price = i.Price,
                        category_id = i.CategoryId,
                        category_name = i.Category != null ? i.Category.CategoryName : null,
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
                        category_name = i.Category != null ? i.Category.CategoryName : null,
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

                // Lấy lại item sau khi thêm (đã include category)
                var createdItem = await _context.Items
                    .Include(i => i.Category)
                    .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

                return new ItemResponseDto
                {
                    item_id = createdItem.ItemId,
                    name = createdItem.Name,
                    description = createdItem.Description,
                    price = createdItem.Price,
                    category_id = createdItem.CategoryId,
                    category_name = createdItem.Category?.CategoryName,
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

                if (!string.IsNullOrEmpty(itemDto.description))
                    existingItem.Description = itemDto.description;

                if (itemDto.price.HasValue)
                    existingItem.Price = itemDto.price.Value;

                if (itemDto.category_id > 0)
                {
                    var categoryExists = await _context.Category.AnyAsync(c => c.CategoryId == itemDto.category_id);
                    if (categoryExists)
                        existingItem.CategoryId = itemDto.category_id;
                }

                if (!string.IsNullOrEmpty(itemDto.image_url))
                    existingItem.ImageUrl = itemDto.image_url;

                existingItem.IsActive = itemDto.is_active;

                existingItem.UpdatedAt = DateTime.Now;

                _context.Items.Update(existingItem);
                await _context.SaveChangesAsync();

                return new ItemResponseDto
                {
                    item_id = existingItem.ItemId,
                    name = existingItem.Name,
                    description = existingItem.Description,
                    price = existingItem.Price,
                    category_id = existingItem.CategoryId,
                    category_name = existingItem.Category?.CategoryName,
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
    }
}