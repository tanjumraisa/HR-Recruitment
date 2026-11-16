using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Personal document
    [DisplayColumn(nameof(Title))]
    public class EmployeeDocument : ISoftDelete
    {
        [Key]
        public int EmployeeDocumentId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Document type"), Required, StringLength(30), Choices("NID|Passport|Birth Certificate|Certificate|Contract|Other")]
        public string DocumentType { get; set; } = "NID";

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Document no"), StringLength(50)]
        public string? DocumentNo { get; set; }

        [Display(Name = "Issue date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Expiry date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
