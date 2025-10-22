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
    }
}
