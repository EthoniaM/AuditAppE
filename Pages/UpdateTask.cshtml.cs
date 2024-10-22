using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class UpdateTaskModel : PageModel
    {
        [BindProperty]
        public Kanban Task { get; set; } = new Kanban();

        public void OnGet(int kanbanId)
        {
            // Set TaskID from URL parameter
            Task.KanBanID = kanbanId;

            // SQL query to retrieve task details
            string sql = "SELECT * FROM Kanban WHERE KanBanID = @KanBanID";
            var parameters = new[]
            {
                new SqlParameter("@KanBanID", kanbanId)
            };

            // Using DataCon to retrieve data
            DataSet dataset = DataCon.BuildDataSet(sql, parameters);
            if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                var row = dataset.Tables[0].Rows[0];
                Task.AuditID = row["AuditID"] as int?;
                Task.TaskTitle = row["TaskTitle"] as string;
                Task.Status = row["Status"] as string;
                Task.Priority = row["Priority"] as string;
                Task.TaskDescription = row["TaskDescription"] as string;
                Task.StartDate = row["StartDate"] as DateTime?;
                Task.EndDate = row["EndDate"] as DateTime?;
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

          
            if (Task.StartDate.HasValue)
            {
                Task.StartDate = Task.StartDate.Value.Date + DateTime.Now.TimeOfDay; // Set current time with user-provided date
            }
            else
            {
                Task.StartDate = DateTime.Now; 
            }


            if (Task.EndDate.HasValue)
            {
                Task.EndDate = Task.EndDate.Value.Date + DateTime.Now.TimeOfDay; 
            }
            else
            {
                Task.EndDate = DateTime.Now; 
            }

            string sql = "UPDATE Kanban SET AuditID = @AuditID, TaskTitle = @TaskTitle, Status = @Status, Priority = @Priority, " +
                         "TaskDescription = @TaskDescription, StartDate = @StartDate, EndDate = @EndDate WHERE KanBanID = @KanBanID";

            var parameters = new[]
            {
        new SqlParameter("@KanBanID", Task.KanBanID),
        new SqlParameter("@AuditID", Task.AuditID ?? (object)DBNull.Value),
        new SqlParameter("@TaskTitle", Task.TaskTitle ?? (object)DBNull.Value),
        new SqlParameter("@Status", Task.Status ?? (object)DBNull.Value),
        new SqlParameter("@Priority", Task.Priority ?? (object)DBNull.Value),
        new SqlParameter("@TaskDescription", Task.TaskDescription ?? (object)DBNull.Value),
        new SqlParameter("@StartDate", Task.StartDate ?? (object)DBNull.Value),
        new SqlParameter("@EndDate", Task.EndDate ?? (object)DBNull.Value)
    };

            try
            {
                DataCon.ExecNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                // Handle and log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

            return RedirectToPage("/Success", new { message = "Task updated successfully!" });
        }
    }
}