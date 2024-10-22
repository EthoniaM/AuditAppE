using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace KanBan.Pages
{
    public class DeleteKanbanDetailModel : PageModel
    {
       
        [BindProperty]
        public KanbanDetail KanbanDetail { get; set; }

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
        }

        public IActionResult OnPost()
        {
            // Delete the KanbanDetail from the database
            string query = "DELETE FROM KanbanDetails WHERE KanbanDetailsID = @KanbanDetailsID";
            SqlParameter[] parameters = { new SqlParameter("@KanbanDetailsID", KanbanDetail.KanbanDetailsID) };

            DataCon.ExecNonQuery(query, parameters);

            return RedirectToPage("/Success", new { message = "Details deleted successfully!" });
        }
    }
}
