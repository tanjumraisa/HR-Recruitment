using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Canteen meal / item
    [DisplayColumn(nameof(Name))]
    public class CanteenItem : ISoftDelete
    {
        [Key]
        public int CanteenItemId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Meal type"), Required, StringLength(20), Choices("Breakfast|Lunch|Snacks|Dinner")]
        public string MealType { get; set; } = "Lunch";

        [Display(Name = "Unit price"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Company subsidy %"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "100")]
        public decimal SubsidyPercent { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
