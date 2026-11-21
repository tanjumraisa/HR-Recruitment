using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Entities
{
    // Which permissions are inside which group.
    public class PermissionGroupMap
    {
        [Key]
        public int PermissionGroupMapId { get; set; }

        public int PermissionGroupId { get; set; }

        public int PermissionId { get; set; }

        public bool IsActive { get; set; } = true;

        public int CompanyId { get; set; } = 1;

        [ForeignKey(nameof(PermissionGroupId)), JsonIgnore] public virtual PermissionGroup PermissionGroup { get; set; } = null!;
        [ForeignKey(nameof(PermissionId)), JsonIgnore] public virtual Permission Permission { get; set; } = null!;
        [ForeignKey(nameof(CompanyId)), JsonIgnore] public virtual Company Company { get; set; } = null!;
    }
}
