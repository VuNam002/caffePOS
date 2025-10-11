using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaffePOS.Model
{
    public class Permissions
    {
        [Key]
        [Column("permission_id")]
        public int Permission { get; set; }

        [Column("permission_name")]
        public string PermissionName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("module")]
        public string? Module { get; set; }

        [Column("create_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
