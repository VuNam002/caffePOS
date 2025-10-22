using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("fullname")]
        public string? FullName { get; set; }

        [ForeignKey(nameof(Role))]
        [Column("role_id")]
        public int RoleId { get; set; }

        [StringLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [StringLength(20)]
        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
