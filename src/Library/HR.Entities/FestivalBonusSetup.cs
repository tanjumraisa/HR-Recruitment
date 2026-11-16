using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Festival bonus rule
    public class FestivalBonusSetup : ISoftDelete
    {
        [Key]
        public int FestivalBonusSetupId { get; set; }

        [Display(Name = "Festival"), Required, StringLength(30), Choices("EidUlFitr|EidUlAdha|DurgaPuja|Christmas|BuddhaPurnima|Other")]
        public string Festival { get; set; } = "EidUlFitr";

        [Display(Name = "Employment type (empty = all)")]
        public int? EmploymentTypeId { get; set; }

        [Display(Name = "Calculation type"), Required, StringLength(20), Choices("PercentOfBasic|DaysOfBasic")]
        public string CalculationType { get; set; } = "PercentOfBasic";

        [Display(Name = "Percent or days"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Value { get; set; }

        [Display(Name = "Min service months"), Range(0, 120)]
        public int MinServiceMonths { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmploymentTypeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual EmploymentType? EmploymentType { get; set; }
    }
}
