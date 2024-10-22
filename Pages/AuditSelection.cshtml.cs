using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace KanBan.Pages
{
    public class AuditSelectionModel : PageModel
    {
        private static DataCon _dataCon = new DataCon();

        [BindProperty]
        public int? SelectedAuditID { get; set; }

        public IList<Audit> Audit { get; set; } = new List<Audit>();

        public async Task OnGetAsync()
        {
            try
            {
                // Load the audits from the database
                var sqlQuery = "SELECT AuditID, AuditTitle FROM Audit"; // Query for the Audit table
                var dataSet = DataCon.BuildDataSet(sqlQuery); // Call static method directly

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        Audit.Add(new Audit
                        {
                            AuditID = (int)row["AuditID"],
                            AuditTitle = row["AuditTitle"].ToString()
                        });
                    }
                }
                else
                {
                    // Handle the case where no data is returned
                    ModelState.AddModelError(string.Empty, "No audits found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ModelState.AddModelError(string.Empty, "An error occurred while loading audits.");
            }
        }

        public IActionResult OnPost()
        {
            if (SelectedAuditID.HasValue)
            {
                // Redirect to the Kanban board with the selected AuditID
                return RedirectToPage("/KanbanBoard", new { auditId = SelectedAuditID });
            }
            else
            {
                // Handle the case where no AuditID was selected
                ModelState.AddModelError(string.Empty, "Please select an audit.");
                return Page();
            }
        }
    }
}
