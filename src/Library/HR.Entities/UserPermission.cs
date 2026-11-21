using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Entities
{
    // Gives a user either a whole group (PermissionGroupId) or one extra permission (PermissionId).
    public class UserPermission
    {
        [Key]
        public int UserPermissionId { get; set; }

        public int CompanyId { get; set; } = 1;

        public int UserId { get; set; }

        public int? PermissionId { get; set; }

        public int? PermissionGroupId { get; set; }

        [ForeignKey(nameof(CompanyId)), JsonIgnore] public virtual Company Company { get; set; } = null!;
        [ForeignKey(nameof(UserId)), JsonIgnore] public virtual User User { get; set; } = null!;
        [ForeignKey(nameof(PermissionId)), JsonIgnore] public virtual Permission? Permission { get; set; }
        [ForeignKey(nameof(PermissionGroupId)), JsonIgnore] public virtual PermissionGroup? PermissionGroup { get; set; }
    }
}
