using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Users
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [StringLength(50)]
        public string? userName { get; set; }
        public string UserName { get; internal set; }
        [Required]
        public string? passWord { get; set; }
        public string Password { get; internal set; }
        public string? fullName { get; set; }
        public string FullName { get; internal set; }
        [ForeignKey("Role")] // Đánh dấu đây là khóa ngoại liên kết đến bảng Role
        public int role_id { get; set; }

        [StringLength(100)]
        public string? email { get; set; }
        public string Email { get; internal set; }
        [StringLength(20)]
        public string? phoneNumber { get; set; }
        public string PhoneNumber { get; internal set; }
        public bool is_active { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? updated_at { get; set; }

        // Thuộc tính điều hướng: Tham chiếu đến đối tượng Role
        public virtual required Role Role { get; set; }

        // Thuộc tính điều hướng: Một người dùng có thể có nhiều đơn hàng
        public virtual required ICollection<Order> Orders { get; set; }
        public int UserId { get; internal set; }
        public int RoleId { get; internal set; }
        public bool? IsActive { get; internal set; }
        public DateTime? CreatedAt { get; internal set; }
        public DateTime? UpdatedAt { get; internal set; }
    }
}
