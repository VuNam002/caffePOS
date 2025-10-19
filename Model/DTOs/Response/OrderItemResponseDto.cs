namespace CaffePOS.Model.DTOs.Response
{
    public class OrderItemResponseDto
    {
        public int order_item_id { get; set; }
        public int item_id { get; set; }
        public int quantity { get; set; }
        public decimal price_at_sale { get; set; }
        public decimal subtotal { get; set; }
        public string? item_notd { get; set; }
    }
}