using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CaffePOS.Model
{
    public class OrderItem
    {
        [Key]
        public int item_id { get; set; }

        [Required]
        public string? name { get; set; }

        public string? description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal price { get; set; }

        [ForeignKey("Category")]
        public int category_id { get; set; }

        public string? image_url { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? updated_at { get; set; }

        // Thuộc tính điều hướng đến danh mục của sản phẩm
        public virtual Category? Category { get; set; }

        // Thuộc tính điều hướng: Một sản phẩm có thể xuất hiện trong nhiều chi tiết đơn hàng
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public int ItemId { get; internal set; }
        public int Quantity { get; internal set; }
        public decimal PriceAtSale { get; internal set; }
        public decimal Subtotal { get; internal set; }
        public string? ItemNotd { get; internal set; }
        public object Item { get; internal set; }
    }
}