using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
