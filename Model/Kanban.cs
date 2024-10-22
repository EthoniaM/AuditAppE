using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KanBan.Model
{
    public class Kanban
    {
        [Key]
        public int KanBanID { get; set; }

        [Required]
        public int? AuditID { get; set; }

        [Required]
        [StringLength(100)]
        public string? TaskTitle { get; set; }

        [Required]
        public string? Status { get; set; } // e.g., Not Started, In Progress, Completed

        [Required]
        public string? Priority { get; set; } // e.g., Low, Medium, High

        public string? TaskDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [ForeignKey("AuditID")]
        public Audit? Audit { get; set; }
    }
}

