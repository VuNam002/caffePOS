using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_at_sale")]
        public decimal PriceAtSale { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("item_notd")]
        public string? ItemNotd {  get; set; }
    }
}
