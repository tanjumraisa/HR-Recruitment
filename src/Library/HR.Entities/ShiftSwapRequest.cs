using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Shift exchange between two employees
    public class ShiftSwapRequest : ISoftDelete
    {
        [Key]
        public int ShiftSwapRequestId { get; set; }

        [Display(Name = "Requested by"), Range(1, int.MaxValue, ErrorMessage = "Please select requested by.")]
        public int RequesterId { get; set; }

        [Display(Name = "Swap with"), Range(1, int.MaxValue, ErrorMessage = "Please select swap with.")]
        public int SwapWithId { get; set; }

        [Display(Name = "Shift"), Range(1, int.MaxValue, ErrorMessage = "Please select shift.")]
        public int ShiftId { get; set; }

        [Display(Name = "Swap date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime SwapDate { get; set; } = DateTime.Today;

        [Display(Name = "Reason"), StringLength(500)]
        public string? Reason { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(RequesterId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Requester { get; set; } = null!;

        [ForeignKey(nameof(SwapWithId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee SwapWith { get; set; } = null!;

        [ForeignKey(nameof(ShiftId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Shift Shift { get; set; } = null!;
    }
}
