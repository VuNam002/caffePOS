using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Permissions
    {
        [Key]
        public int permission_id { get; set; }

        [Required]
        [StringLength(50)]
        public string? permission_name { get; set; }

        public string? description { get; set; }

        [StringLength(50)]
        public required string module { get; set; }

        public DateTime create_at { get; set; }

        public virtual required ICollection<RolePermission> RolePermissions { get; set; }
    }
}
