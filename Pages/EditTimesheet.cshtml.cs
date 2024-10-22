using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using KanBan.Model;

namespace KanBan.Pages
{
    public class EditTimesheetModel : PageModel
    {
        [BindProperty]
        public Timesheet Timesheet { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public IActionResult OnGet(int id)
        {
            // Query to get the timesheet record
            string sqlQuery = "SELECT * FROM Timesheet WHERE TimesheetID = @TimesheetID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TimesheetID", id)
            };

            var ds = DataCon.BuildDataSet(sqlQuery, parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                Timesheet = new Timesheet
                {
                    TimesheetID = id,
                    Date = DateOnly.FromDateTime(Convert.ToDateTime(row["Date"])), // Update this line
                    Hours = row["Hours"].ToString(),
                    Activity = row["Activity"].ToString(),
                    IsAudit = Convert.ToBoolean(row["IsAudit"]),
                    AuditPhase = row["AuditPhase"].ToString(),
                    UsersID = Convert.ToInt32(row["UsersID"]),
                  
                    AuditID = row["AuditID"] as int?
                };
                return Page();
            }
            return NotFound();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // SQL query to update the timesheet record
            string sqlUpdate = "UPDATE Timesheet SET Date = @Date, Hours = @Hours, Activity = @Activity, " +
                               "IsAudit = @IsAudit, AuditPhase = @AuditPhase, UsersID = @UsersID, " +
                               " AuditID = @AuditID " +
                               "WHERE TimesheetID = @TimesheetID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Date", Timesheet.Date.ToDateTime(new TimeOnly(0, 0))),
                new SqlParameter("@Hours", Timesheet.Hours),
                new SqlParameter("@Activity", Timesheet.Activity),
                new SqlParameter("@IsAudit", Timesheet.IsAudit),
                new SqlParameter("@AuditPhase", Timesheet.AuditPhase ?? (object)DBNull.Value), 
                new SqlParameter("@UsersID", Timesheet.UsersID),
             
                new SqlParameter("@AuditID", (object)Timesheet.AuditID ?? DBNull.Value), 
                new SqlParameter("@TimesheetID", Timesheet.TimesheetID)
            };

            try
            {
                DataCon.ExecNonQuery(sqlUpdate, parameters);
                return RedirectToPage("/Success", new { message = "Timesheet updated successfully!" });
            }
            catch
            {
                return StatusCode(500); // Handle failure
            }
        }
    }
}
