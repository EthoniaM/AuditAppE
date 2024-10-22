using KanBan.Pages;

namespace KanBan.Model
{
    public class Timesheet
    {
        public int TimesheetID { get; set; }
     
        public string Activity { get; set; }
        public bool IsAudit { get; set; }
        public int? AuditID { get; set; }
        public string AuditPhase { get; set; }
        public string Hours { get; set; }
        public DateOnly Date { get; set; }
        public int UsersID { get; set; }
    }
}
