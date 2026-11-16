using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // House rent, medical, PF ...
    [DisplayColumn(nameof(Name))]
    public class SalaryComponent : ISoftDelete
    {
        [Key]
        public int SalaryComponentId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Component type"), Required, StringLength(20), Choices("Earning|Deduction")]
        public string ComponentType { get; set; } = "Earning";

        [Display(Name = "Calculation type"), Required, StringLength(20), Choices("Fixed|PercentOfBasic")]
        public string CalculationType { get; set; } = "Fixed";

        [Display(Name = "Default value"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal DefaultValue { get; set; }

        [Display(Name = "Taxable")]
        public bool IsTaxable { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
