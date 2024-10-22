using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace KanBan.Pages
{
    public class EditKanbanDetailModel : PageModel
    {
        public EditKanbanDetailModel()
        {
            // Initialize properties if needed
        }

        [BindProperty]
        public KanbanDetail KanbanDetail { get; set; }

        // List of Kanbans and users for dropdowns
        public IEnumerable<SelectListItem> KanbanSelectList { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> UsersSelectList { get; set; } = Enumerable.Empty<SelectListItem>();

        public void OnGet(int id)
        {
            // Fetch KanbanDetail from database
            string query = "SELECT * FROM KanbanDetails WHERE KanbanDetailsID = @KanbanDetailsID";
            SqlParameter[] parameters = { new SqlParameter("@KanbanDetailsID", id) };
            DataSet dataSet = DataCon.BuildDataSet(query, parameters);

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = dataSet.Tables[0].Rows[0];
                KanbanDetail = new KanbanDetail
                {
                    KanbanDetailsID = (int)row["KanbanDetailsID"],
                    KanbanID = (int)row["KanbanID"],
                    UsersID = (int)row["UsersID"],
                    Comments = row["Comments"].ToString()
                };
            }

            // Populate dropdowns
            PopulateDropdowns();
        }

        private void PopulateDropdowns()
        {
            // Fetch Kanbans and users from the database using DataCon
            string kanbanQuery = "SELECT KanbanID, TaskTitle FROM Kanban"; // Adjust query as needed
            string usersQuery = "SELECT UsersID, CONCAT(FirstName, ' ', LastName) AS FullName FROM Users"; // Adjust query as needed

            DataSet kanbanDataSet = DataCon.BuildDataSet(kanbanQuery);
            DataSet userDataSet = DataCon.BuildDataSet(usersQuery);

            KanbanSelectList = kanbanDataSet.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row["KanbanID"].ToString(),
                Text = row["TaskTitle"].ToString() // Adjust text as needed
            });

            UsersSelectList = userDataSet.Tables[0].AsEnumerable().Select(row => new SelectListItem
            {
                Value = row["UsersID"].ToString(),
                Text = row["FullName"].ToString() // Adjust text as needed
            });
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate the dropdowns in case of validation errors
                PopulateDropdowns();
                return Page();
            }

            // Update the KanbanDetail in the database
            string query = "UPDATE KanbanDetails SET KanbanID = @KanbanID, UsersID = @UsersID, Comments = @Comments WHERE KanbanDetailsID = @KanbanDetailsID";
            SqlParameter[] parameters = {
                new SqlParameter("@KanbanID", KanbanDetail.KanbanID),
                new SqlParameter("@UsersID", KanbanDetail.UsersID),
                new SqlParameter("@Comments", KanbanDetail.Comments),
                new SqlParameter("@KanbanDetailsID", KanbanDetail.KanbanDetailsID)
            };

            DataCon.ExecNonQuery(query, parameters);


            return RedirectToPage("/Success", new { message = "Details Updated successfully!" });
        }
    }
}
