using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace KanBan.Pages
{
    public class ActivityandAuditPhaseModel : PageModel
    {
        [BindProperty]
        public string Activity { get; set; }

        [BindProperty]
        public string AuditPhase { get; set; }

        public List<TimesheetActivity> TimesheetActivities { get; set; } = new List<TimesheetActivity>();
        public List<TimesheetAuditPhase> TimesheetAuditPhases { get; set; } = new List<TimesheetAuditPhase>();

        public void OnGet()
        {
            LoadCurrentActivities();
            LoadCurrentAuditPhases();
        }

        // For the 'AddActivity' form
        public IActionResult OnPostAddActivity()
        {
            // Only validate the Activity property for this handler
            ModelState.Remove(nameof(AuditPhase)); // Remove validation for AuditPhase

            if (!ModelState.IsValid)
            {
                LoadCurrentActivities();
                LoadCurrentAuditPhases();
                return Page();
            }

            // Insert into TimesheetActivity table
            string query = "INSERT INTO TimesheetActivity (Activity) VALUES (@Activity)";
            SqlParameter[] parameters = {
                new SqlParameter("@Activity", Activity)
            };
            DataCon.ExecNonQuery(query, parameters);

            // Clear the Activity property after adding it
            Activity = string.Empty;

            return RedirectToPage("/Success", new { message = "Activities added successfully!" });
        }

        // For the 'AddAuditPhase' form
        public IActionResult OnPostAddAuditPhase()
        {
            // Only validate the AuditPhase property for this handler
            ModelState.Remove(nameof(Activity)); // Remove validation for Activity

            if (!ModelState.IsValid)
            {
                LoadCurrentActivities();
                LoadCurrentAuditPhases();
                return Page();
            }

            // Insert into TimesheetAuditPhase table
            string query = "INSERT INTO TimesheetAuditPhase (AuditPhase) VALUES (@AuditPhase)";
            SqlParameter[] parameters = {
                new SqlParameter("@AuditPhase", AuditPhase)
            };
            DataCon.ExecNonQuery(query, parameters);

            // Clear the AuditPhase property after adding it
            AuditPhase = string.Empty;

            return RedirectToPage("/Success", new { message = "AuditPhase added successfully!" });
        }

        private void LoadCurrentActivities()
        {
            // Load current activities from the database
            string query = "SELECT * FROM TimesheetActivity";
            DataSet dataSet = DataCon.BuildDataSet(query);
            TimesheetActivities = dataSet.Tables[0].AsEnumerable().Select(row => new TimesheetActivity
            {
                TimeSheetActivityID = row.Field<int>("TimeSheetActivityID"),
                Activity = row.Field<string>("Activity")
            }).ToList();
        }

        private void LoadCurrentAuditPhases()
        {
            // Load current audit phases from the database
            string query = "SELECT * FROM TimesheetAuditPhase";
            DataSet dataSet = DataCon.BuildDataSet(query);
            TimesheetAuditPhases = dataSet.Tables[0].AsEnumerable().Select(row => new TimesheetAuditPhase
            {
                TimeSheetAuditPhaseID = row.Field<int>("TimeSheetAuditPhaseID"),
                AuditPhase = row.Field<string>("AuditPhase")
            }).ToList();
        }
    }

    public class TimesheetActivity
    {
        public int TimeSheetActivityID { get; set; }
        public string Activity { get; set; }
    }

    public class TimesheetAuditPhase
    {
        public int TimeSheetAuditPhaseID { get; set; }
        public string AuditPhase { get; set; }
    }
}
