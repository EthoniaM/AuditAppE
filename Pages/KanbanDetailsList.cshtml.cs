using KanBan.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace KanBan.Pages
{
    public class KanbanDetailsListModel : PageModel
    {
        public KanbanDetailsListModel()
        {
            // Initialize properties if needed
        }

        public IList<KanbanDetail> KanbanDetailsList { get; set; } = new List<KanbanDetail>();

        public void OnGet()
        {
            // Fetch all KanbanDetails from the database
            string query = "SELECT * FROM KanbanDetails";
            DataSet dataSet = DataCon.BuildDataSet(query);

            KanbanDetailsList = dataSet.Tables[0].AsEnumerable().Select(row => new KanbanDetail
            {
                KanbanDetailsID = (int)row["KanbanDetailsID"],
                KanbanID = (int)row["KanbanID"],
                UsersID = (int)row["UsersID"],
                Comments = row["Comments"].ToString()
            }).ToList();
        }
    }
}
