using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using KanBan.Model;

namespace KanBan.Pages
{
    public class DeleteTimesheetModel : PageModel
    {
        public Timesheet Timesheet { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public IActionResult OnGet(int id)
        {
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
                  
                    Date = DateOnly.FromDateTime(Convert.ToDateTime(row["Date"])), 
                    Hours = row["Hours"].ToString(),
                    Activity = row["Activity"].ToString(),
                };
                return Page();
            }
            return NotFound();
        }

        public IActionResult OnPost()
        {
            string sqlDelete = "DELETE FROM Timesheet WHERE TimesheetID = @TimesheetID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TimesheetID", id)
            };

            try
            {
                DataCon.ExecNonQuery(sqlDelete, parameters);
                return RedirectToPage("/Success", new { message = "Timesheet deleted successfully!" }); // Updated message
            }
            catch
            {
                return StatusCode(500); // Handle failure
            }
        }
    }
}
