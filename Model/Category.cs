using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }

        [Required]
        public string category_name { get; set; }

        public string description { get; set; }
        public string Description { get; internal set; }
        public DateTime created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public bool is_active { get; set; }

        // Thuộc tính điều hướng: Một danh mục có nhiều sản phẩm
        public virtual ICollection<Item> Items { get; set; }
        public string CategoryName { get; internal set; }
        public int CategoryId { get; internal set; }
        public bool IsActive { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}