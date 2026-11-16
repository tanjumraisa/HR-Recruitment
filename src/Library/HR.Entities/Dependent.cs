using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Family member / nominee
    [DisplayColumn(nameof(Name))]
    public class Dependent : ISoftDelete
    {
        [Key]
        public int DependentId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Relationship"), Required, StringLength(20), Choices("Spouse|Child|Father|Mother|Other")]
        public string Relationship { get; set; } = "Spouse";

        [Display(Name = "Date of birth"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender"), Required, StringLength(10), Choices("Male|Female|Other")]
        public string Gender { get; set; } = "Male";

        [Display(Name = "Nominee")]
        public bool IsNominee { get; set; }

        [Display(Name = "Nominee share %"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "100")]
        public decimal NomineeSharePercent { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
