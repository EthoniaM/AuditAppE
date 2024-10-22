using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using KanBan.Model;

namespace KanBan.Pages
{
    public class ManageTimesheetModel : PageModel
    {
        public List<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

        public void OnGet()
        {
            LoadTimesheets();
        }

        private void LoadTimesheets()
        {
            string query = "SELECT * FROM Timesheet"; 
            try
            {
                DataSet ds = DataCon.BuildDataSet(query);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Timesheets.Add(new Timesheet
                    {
                        TimesheetID = Convert.ToInt32(row["TimesheetID"]),
                        Date = DateOnly.FromDateTime(Convert.ToDateTime(row["Date"])),
                        Hours = row["Hours"].ToString(),
                        Activity = row["Activity"].ToString(),
                        IsAudit = Convert.ToBoolean(row["IsAudit"]),
                        AuditPhase = row["AuditPhase"].ToString(),
                        UsersID = Convert.ToInt32(row["UsersID"]),
                        AuditID = row["AuditID"] as int?
                    });
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine(ex.Message);
            }
        }

    }
}
