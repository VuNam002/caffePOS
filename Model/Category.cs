using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; } 

        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;  

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;  
    }
}