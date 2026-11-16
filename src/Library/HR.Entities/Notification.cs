using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Message to a user
    [DisplayColumn(nameof(Title))]
    public class Notification : ISoftDelete
    {
        [Key]
        public int NotificationId { get; set; }

        [Display(Name = "User"), Range(1, int.MaxValue, ErrorMessage = "Please select user.")]
        public int UserId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Message"), StringLength(500)]
        public string? Message { get; set; }

        [Display(Name = "Link"), StringLength(300)]
        public string? Link { get; set; }

        [Display(Name = "Read")]
        public bool IsRead { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}
