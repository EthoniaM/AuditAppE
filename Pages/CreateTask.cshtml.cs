using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KanBan.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class CreateTaskModel : PageModel
    {
        [BindProperty]
     
        public int? AuditID { get; set; }
        [BindProperty]
        public string TaskTitle { get; set; }
        [BindProperty]
        public string Status { get; set; }
        [BindProperty]
        public string Priority { get; set; }
        [BindProperty]
        public string TaskDescription { get; set; }
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }

        public void OnGet(int? auditId)
        {
            AuditID = auditId;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // If the user has selected a StartDate, set the time to the current time
            if (StartDate.HasValue)
            {
                StartDate = StartDate.Value.Date + DateTime.Now.TimeOfDay; // Set current time with user-provided date
            }
            else
            {
                StartDate = DateTime.Now; // Default to current date and time if no date is provided
            }

            // If the user has selected an EndDate, set the time to the current time
            if (EndDate.HasValue)
            {
                EndDate = EndDate.Value.Date + DateTime.Now.TimeOfDay; // Set current time with user-provided date
            }
            else
            {
                EndDate = DateTime.Now; // Default to current date and time if no date is provided
            }

            var sql = "INSERT INTO Kanban (AuditID, TaskTitle, Status, Priority, TaskDescription, StartDate, EndDate) " +
                      "VALUES (@AuditID, @TaskTitle, @Status, @Priority, @TaskDescription, @StartDate, @EndDate)";

            var parameters = new[]
            {
        new SqlParameter("@AuditID", AuditID ?? (object)DBNull.Value),
        new SqlParameter("@TaskTitle", TaskTitle ?? (object)DBNull.Value),
        new SqlParameter("@Status", Status ?? (object)DBNull.Value),
        new SqlParameter("@Priority", Priority ?? (object)DBNull.Value),
        new SqlParameter("@TaskDescription", TaskDescription ?? (object)DBNull.Value),
        new SqlParameter("@StartDate", StartDate ?? (object)DBNull.Value),
        new SqlParameter("@EndDate", EndDate ?? (object)DBNull.Value)
    };

            try
            {
                DataCon.ExecNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                // Log exception details
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

            return RedirectToPage("/KanbanBoard", new { auditId = AuditID });
        }


    }
}
