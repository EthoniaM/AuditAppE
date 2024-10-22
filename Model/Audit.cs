using System.ComponentModel.DataAnnotations;


namespace KanBan.Model
{
    public class Audit
    {
        [Key]
        public int AuditID { get; set; }

        public string? AuditTitle { get; set; }
        public string? AuditDescription { get; set; }

        public virtual ICollection<Kanban> Kanbans { get; set; } = new List<Kanban>();
    }
}
