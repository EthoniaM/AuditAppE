using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class CreateAuditModel : PageModel
    {
        [BindProperty]
        public Audit Audit { get; set; } = new Audit();

        public void OnGet()
        {
            // Initialize any properties if necessary
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Prepare the SQL query and parameters
            string sqlQuery = "INSERT INTO Audit (AuditTitle, AuditDescription, CreatedOn) VALUES (@AuditTitle, @AuditDescription, @CreatedOn)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AuditTitle", Audit.AuditTitle),
                new SqlParameter("@AuditDescription", Audit.AuditDescription),
                new SqlParameter("@CreatedOn", DateTime.Now)
            };

            // Execute the SQL command
            try
            {
                DataCon.ExecNonQuery(sqlQuery, parameters);
                return RedirectToPage("/Success", new { message = "Audit Created successfully!" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the audit: {ex.Message}");
                return Page();
            }
        }
    }
}