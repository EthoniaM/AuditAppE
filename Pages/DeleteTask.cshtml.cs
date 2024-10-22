using KanBan;
using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class DeleteTaskModel : PageModel
    {
        public DeleteTaskModel()
        {
            Kanban = new Kanban(); 
        }

        [BindProperty]
        public Kanban Kanban { get; set; }

        public void OnGet(int id)
        {
            
            Kanban = new Kanban();

            string query = "SELECT * FROM Kanban WHERE KanBanID = @KanBanID";
            SqlParameter[] parameters = { new SqlParameter("@KanBanID", id) };
            var dataSet = DataCon.BuildDataSet(query, parameters);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var row = dataSet.Tables[0].Rows[0];
                Kanban = new Kanban
                {
                    KanBanID = (int)row["KanBanID"],
                    AuditID = row["AuditID"] == DBNull.Value ? (int?)null : (int)row["AuditID"],
                    TaskTitle = row["TaskTitle"].ToString(),
                    Status = row["Status"].ToString(),
                    Priority = row["Priority"].ToString(),
                    TaskDescription = row["TaskDescription"].ToString(),
                    StartDate = row["StartDate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["StartDate"],
                    EndDate = row["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime)row["EndDate"]
                };
            }
            else
            {
                RedirectToPage("/Error", new { message = "Task not found." });
            }
        }

        public IActionResult OnPost()
        {
            if (Kanban.KanBanID <= 0)
            {
                return RedirectToPage("/Error", new { message = "Invalid task ID." });
            }

            string query = "DELETE FROM Kanban WHERE KanBanID = @KanBanID";
            SqlParameter[] parameters = { new SqlParameter("@KanBanID", Kanban.KanBanID) };

            try
            {
                DataCon.ExecNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message); // Replace with proper logging
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

            return RedirectToPage("/Success", new { message = "Task deleted successfully!" });
        }
    }
}
