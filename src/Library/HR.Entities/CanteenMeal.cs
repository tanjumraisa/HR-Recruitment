using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Meal taken by an employee
    public class CanteenMeal : ISoftDelete
    {
        [Key]
        public int CanteenMealId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Item"), Range(1, int.MaxValue, ErrorMessage = "Please select item.")]
        public int CanteenItemId { get; set; }

        [Display(Name = "Date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Quantity"), Range(1, 20)]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Employee pays (calculated)"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(CanteenItemId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual CanteenItem CanteenItem { get; set; } = null!;
    }
}
