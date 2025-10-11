using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Users
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("userName")]
        public string UserName { get; set; } = string.Empty;

        [Column("passWord")]
        public string? Password { get; set; }

        [Column("fullName")]
        public string? FullName { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? Email { get; set; }

        [Column("phoneNumber")]
        public int PhoneNumber { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
