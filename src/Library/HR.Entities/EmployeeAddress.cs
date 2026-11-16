using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Present / permanent address
    [DisplayColumn(nameof(AddressLine))]
    public class EmployeeAddress : ISoftDelete
    {
        [Key]
        public int EmployeeAddressId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Address type"), Required, StringLength(20), Choices("Present|Permanent")]
        public string AddressType { get; set; } = "Present";

        [Display(Name = "Address line"), Required, StringLength(250)]
        public string AddressLine { get; set; } = string.Empty;

        [Display(Name = "City"), StringLength(60)]
        public string? City { get; set; }

        [Display(Name = "District"), StringLength(60)]
        public string? District { get; set; }

        [Display(Name = "Post code"), StringLength(10)]
        public string? PostCode { get; set; }

        [Display(Name = "Country"), StringLength(60)]
        public string? Country { get; set; } = "Bangladesh";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
