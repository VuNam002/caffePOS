using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Payments
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("method")]
        public string? Method { get; set; }

        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("notes")]
        public string? Nodes { get; set; }
    }
}
