using CaffePOS.Data;
using CaffePOS.Model;
using CaffePOS.Model.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace CaffePOS.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(AppDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Lấy toàn bộ các phương thức thanh toán
        public async Task<List<PaymentResponseDto>> GetAllPayment()
        {
            try
            {
                return await _context.Payments
                    .Select(p => new PaymentResponseDto
                    {
                        payment_id = p.PaymentId,
                        order_id = p.OrderId,
                        payment_date = p.PaymentDate,
                        amount = p.Amount,
                        method = p.Method,
                        transaction_id = p.TransactionId,
                        notes = p.Notes
                    }).ToListAsync();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay cac phuong thuc thanh toan.");
                throw;
            }
        }
        //Them mới các phương thức thanh toán
        public async Task<PaymentResponseDto> CreatePayment(PaymentPostDto createDto)
        {
            try
            {
                var payment = new Payments
                {
                    OrderId = createDto.order_id,
                    Amount = createDto.amount,
                    Method = createDto.method,
                    Notes = createDto.notes,
                    TransactionId = Guid.NewGuid().ToString()
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                return new PaymentResponseDto
                {
                    payment_id = payment.PaymentId,
                    order_id = payment.OrderId,
                    payment_date = payment.PaymentDate,
                    amount = payment.Amount,
                    method = payment.Method,
                    transaction_id = payment.TransactionId,
                    notes = payment.Notes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi tao moi phuong thuc thanh toan.");
                throw;
            }
        }
        //Lấy chi tiết danh mục thanh toán theo ID
        public async Task<PaymentResponseDto?> Detail(int id)
        {
            try
            {
                var order = await _context.Payments
                    .Where(p => p.PaymentId == id)
                    .Select(p => new PaymentResponseDto
                    {
                        payment_id = p.PaymentId,
                        order_id = p.OrderId,
                        payment_date = p.PaymentDate,
                        amount = p.Amount,
                        method = p.Method,
                        transaction_id = p.TransactionId,
                        notes = p.Notes
                    }).FirstOrDefaultAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay chi tiet phuong thuc thanh toan theo ID.");
                throw;
            }
        }
        //Sua danh muc
        public async Task<PaymentResponseDto?> EditPayment(int id, PaymentPostDto paymentDto)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning("Khong tim thay phuong thuc thanh toan cap nhat");
                    return null;
                }
                payment.Method = paymentDto.method;
                payment.PaymentDate = payment.PaymentDate;
                payment.Amount = payment.Amount;
                payment.Notes = payment.Notes;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da cap nhat phuong thuc thanh toan thanh cong");

                return new PaymentResponseDto
                {
                    payment_id = payment.PaymentId,
                    payment_date = payment.PaymentDate,
                    amount = payment.Amount,
                    method = payment.Method,
                    notes = payment.Notes,
                    order_id = payment.OrderId,
                    transaction_id = payment.TransactionId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi cap nhat phuong thuc thanh toan.");
                throw;
            }
        }
        public async Task<bool> DeletePayment(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if(payment == null)
                {
                    _logger.LogWarning("Khong tim thay phuong thuc thanh toan");
                    return false;
                }
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Da xoa phuong thuc ID: {id}", id);
                return true;
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Co loi khi xoa phuong thuc thanh toan");
                throw;
            }
        }
    }
}
