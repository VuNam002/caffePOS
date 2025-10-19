using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Lấy toàn bộ danh sách đơn hàng
        public async Task<List<OrderResponseDto>> GetAllOrder()
        {
            try
            {
                return await _context.Order
                    .Select(static o => new OrderResponseDto
                    {
                        order_id = o.OrderId,
                        order_date = o.OrderDate,
                        total_amount = o.TotalAmount,
                        discount_amount = (decimal)o.DiscountAmount,
                        final_amount = o.TotalAmount - (o.TotalAmount * (o.DiscountAmount ?? 0) / 100),
                        payment_method = o.PaymentMethod,
                        status = o.Status,
                        notes = o.Notes,
                        user_id = o.UserId,
                        customer_name = o.CustomerName
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all orders.");
                throw;
            }
        }
        //Lấy chi tiết đơn hàng theo ID
        public async Task<OrderResponseDto?> Detail(int id)
        {
            try
            {
                var order = await _context.Order
                    .Where(o => o.OrderId == id)
                    .Select(o => new OrderResponseDto
                    {
                        order_id = o.OrderId,
                        order_date = o.OrderDate,
                        total_amount = o.TotalAmount,
                        discount_amount = o.DiscountAmount ?? 0,
                        final_amount = o.FinalAmount,
                        payment_method = o.PaymentMethod,
                        status = o.Status,
                        notes = o.Notes,
                        user_id = o.UserId,
                        customer_name = o.CustomerName
                    })
                    .FirstOrDefaultAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết đơn hàng ID: {Id}", id);
                throw;
            }
        }
        //Thêm đơn hàng mới
        public async Task<OrderResponseDto> CreateOrder(CreateOrderRequestDto requestDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (requestDto?.Items == null || !requestDto.Items.Any())
                {
                    throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");
                }

                var itemIds = requestDto.Items.Select(i => i.ItemId).Distinct().ToList();

                var itemsFromDb = await _context.Items
                    .Where(p => itemIds.Contains(p.ItemId))
                    .ToDictionaryAsync(p => p.ItemId);
                if (itemsFromDb.Count != itemIds.Count)
                {
                    var missingIds = itemIds.Except(itemsFromDb.Keys);
                    throw new KeyNotFoundException($"Các sản phẩm với ID sau không tồn tại: {string.Join(", ", missingIds)}");
                }

                decimal totalAmount = 0;
                var orderItemsEntities = new List<OrderItem>();

                foreach (var itemDto in requestDto.Items)
                {
                    var item = itemsFromDb[itemDto.ItemId];

                    decimal subtotal = item.Price * itemDto.Quantity;
                    totalAmount += subtotal;

                    orderItemsEntities.Add(new OrderItem
                    {
                        ItemId = item.ItemId,
                        Quantity = itemDto.Quantity,
                        PriceAtSale = item.Price,
                        Subtotal = subtotal,
                        ItemNotd = itemDto.Note
                    });
                }

                // 4. Thống nhất logic tính toán: DiscountAmount là phần trăm (%)
                decimal finalAmount = totalAmount - (totalAmount * (requestDto.DiscountAmount ?? 0) / 100);
                if (finalAmount < 0) finalAmount = 0;

                var order = new Order
                {
                    UserId = requestDto.UserId,
                    CustomerName = requestDto.CustomerName,
                    OrderDate = DateTime.Now,
                    PaymentMethod = requestDto.PaymentMethod,
                    Status = "Pending",
                    Notes = requestDto.Notes,
                    TotalAmount = totalAmount,
                    DiscountAmount = (decimal)requestDto.DiscountAmount,
                    FinalAmount = finalAmount,
                    OrderItems = orderItemsEntities
                };
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponseDto
                {
                    order_id = order.OrderId,
                    order_date = order.OrderDate,
                    total_amount = order.TotalAmount,
                    discount_amount = order.DiscountAmount ?? 0,
                    final_amount = order.FinalAmount,
                    payment_method = order.PaymentMethod,
                    status = order.Status,
                    notes = order.Notes,
                    user_id = order.UserId,
                    customer_name = order.CustomerName,
                    items = order.OrderItems?.Select(oi => new OrderItemResponseDto
                    {
                        order_item_id = oi.OrderItemId,
                        item_id = oi.ItemId,
                        quantity = oi.Quantity,
                        price_at_sale = oi.PriceAtSale,
                        subtotal = oi.Subtotal,
                        item_notd = oi.ItemNotd
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng. Transaction đã được rollback.");
                throw;
            }
        }
        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Order.FindAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Khong tim thay don hang");
                    return false;
                }
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da xoa danh muc ID {id} thanh cong", id);
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Co loi khi xoa don hang");
                throw;
            }
        }
    }

}
