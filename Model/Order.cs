using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CaffePOS.Model
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("customer_name")]
        public string CustomerName { get; set; }

        [Column("discount_amount")]
        public decimal? DiscountAmount { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("final_amount")]
        public decimal FinalAmount { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        // Thuộc tính điều hướng đến người dùng đã tạo đơn hàng
        public virtual Users? User { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có nhiều chi tiết đơn hàng (OrderItem)
        public virtual ICollection<OrderItem>? OrderItems { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có thể có nhiều giao dịch thanh toán
        public virtual ICollection<Payments>? Payments { get; set; }
    }
}