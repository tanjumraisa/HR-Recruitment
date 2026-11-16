using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Income tax slab
    [DisplayColumn(nameof(Description))]
    public class TaxSlab : ISoftDelete
    {
        [Key]
        public int TaxSlabId { get; set; }

        [Display(Name = "Description"), Required, StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Fiscal year"), Required, StringLength(9)]
        public string FiscalYear { get; set; } = string.Empty;

        [Display(Name = "From amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal FromAmount { get; set; }

        [Display(Name = "To amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal ToAmount { get; set; }

        [Display(Name = "Rate percent"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "100")]
        public decimal RatePercent { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
