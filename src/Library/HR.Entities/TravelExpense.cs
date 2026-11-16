using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // TA/DA travel expense claim
    public class TravelExpense : ISoftDelete
    {
        [Key]
        public int TravelExpenseId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Travel date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime TravelDate { get; set; } = DateTime.Today;

        [Display(Name = "From place"), Required, StringLength(100)]
        public string FromPlace { get; set; } = string.Empty;

        [Display(Name = "To place"), Required, StringLength(100)]
        public string ToPlace { get; set; } = string.Empty;

        [Display(Name = "Purpose"), Required, StringLength(250)]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Transport cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal TransportCost { get; set; }

        [Display(Name = "Food cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal FoodCost { get; set; }

        [Display(Name = "Hotel cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal HotelCost { get; set; }

        [Display(Name = "Other cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal OtherCost { get; set; }

        [Display(Name = "Total (calculated)"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected|Paid")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Approved by")]
        public int? ApprovedById { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(ApprovedById))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? ApprovedBy { get; set; }
    }
}
