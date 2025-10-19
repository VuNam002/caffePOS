using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Payments
    {
        [Key]
        public int payment_id { get; set; }

        [ForeignKey("Order")]
        public int order_id { get; set; }

        public DateTime payment_date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal amount { get; set; }

        public string? method { get; set; }

        public string? transaction_id { get; set; }

        public string? notes { get; set; }

        // Thuộc tính điều hướng đến đơn hàng được thanh toán
        public virtual Order? Order { get; set; }
    }
}
