using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CaffePOS.Model
{
    public class Order
    {
        [Key]
        public int order_id { get; set; }

        public DateTime order_date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal total_amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? discount_amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal final_amount { get; set; }

        public string? payment_method { get; set; }

        public string? status { get; set; }
        public string Status { get; internal set; }
        public string? notes { get; set; }
        public string Notes { get; internal set; }
        [ForeignKey("User")]
        public int user_id { get; set; }

        public string? customer_name { get; set; }

        // Thuộc tính điều hướng đến người dùng đã tạo đơn hàng
        public virtual User? User { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có nhiều chi tiết đơn hàng (OrderItem)
        public virtual ICollection<OrderItem>? OrderItems { get; set; }

        // Thuộc tính điều hướng: Một đơn hàng có thể có nhiều giao dịch thanh toán
        public virtual ICollection<Payment>? Payments { get; set; }
        public int OrderId { get; internal set; }
        public DateTime? OrderDate { get; internal set; }
        public decimal TotalAmount { get; internal set; }
        public decimal? DiscountAmount { get; internal set; }
        public string PaymentMethod { get; internal set; }
        public int UserId { get; internal set; }
        public string CustomerName { get; internal set; }
        public decimal FinalAmount { get; internal set; }
    }
}