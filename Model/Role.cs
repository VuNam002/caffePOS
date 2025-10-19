using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
namespace CaffePOS.Model
{
    public class Role
    {
        [Key]
        public int role_id { get; set; }

        [Required] 
        [StringLength(50)] 
        public string? role_name { get; set; }

        public string? description { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? updated_at { get; set; }

        // Thuộc tính điều hướng: Một vai trò có thể có nhiều người dùng
        public virtual ICollection<User> Users { get; set; }

        // Thuộc tính điều hướng: Mối quan hệ nhiều-nhiều với Permissions thông qua bảng RolePermissions
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}

