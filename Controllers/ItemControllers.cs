using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemsService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(ItemsService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemResponseDto>>> GetAll()
        {
            try
            {
                var items = await _itemService.GetAllItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy API");
                return StatusCode(500, "Đã có lỗi khi lấy danh sách sản phẩm");
            }
        }

        //Lấy chi tiết sản phẩm
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> Detail(int id) 
        {
            try
            {
                var item = await _itemService.Detail(id); 

                if (item == null)
                {
                    return NotFound($"Không tìm thấy sản phẩm với id {id}");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết sản phẩm {id}");
                return StatusCode(500, "Đã có lỗi khi lấy chi tiết sản phẩm");
            }
        }

        //Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemsPostDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Dữ liệu sản phẩm không hợp lệ");

                var item = new Items
                {
                    Name = dto.name,
                    Description = dto.description,
                    Price = dto.price ?? 0,
                    CategoryId = dto.category_id,
                    ImageUrl = dto.image_url,
                    IsActive = dto.is_active
                };

                var createdItem = await _itemService.CreateItem(item);

                return CreatedAtAction(nameof(Detail), new { id = createdItem.item_id }, createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm mới");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
        //Sửa sản phẩm
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditItem(int id, [FromBody] ItemsPostDto itemDto)
        {
            if (itemDto == null)
                return BadRequest("Dữ liệu không hợp lệ");

            var updatedItem = await _itemService.EditItem(id, itemDto);

            if (updatedItem == null)
                return NotFound($"Không tìm thấy sản phẩm có id = {id}");

            return Ok(updatedItem);
        }
    }
}