using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employee survey
    [DisplayColumn(nameof(Title))]
    public class Survey : ISoftDelete
    {
        [Key]
        public int SurveyId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Start date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Description"), StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Draft|Open|Closed")]
        public string Status { get; set; } = "Draft";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
