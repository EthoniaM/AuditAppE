using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using KanBan.Model;

namespace KanBan.Pages
{
    public class TimesheetModel : PageModel
    {
        [BindProperty]
        public DateOnly WeekDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [BindProperty]
        public int? SelectedTimeSheetActivityID { get; set; }

        [BindProperty]
      
        public int? SelectedAuditID { get; set; }


        [BindProperty]
        public bool IsAudit { get; set; }

        [BindProperty]
        public List<Timesheet> Timesheets { get; set; } = new List<Timesheet>();

        public List<SelectListItem> TimeSheetAuditPhaseOptions { get; set; }

        public int UsersID { get; set; } // UsersID will now be set dynamically

        public void OnGet()
        {
            UsersID = GetLoggedInUsersID();
            var today = DateOnly.FromDateTime(DateTime.Today);
            int daysSinceSunday = (int)today.DayOfWeek;
            WeekDate = today.AddDays(-daysSinceSunday);

            LoadTimeSheetAuditPhaseOptions();

           
            Timesheets.Clear();

            for (int day = 0; day < 7; day++)
            {
                Timesheets.Add(new Timesheet
                {
                    Date = WeekDate.AddDays(day),
                    Hours = "", 
                    Activity = "",
                    IsAudit = true,
                    AuditID = SelectedAuditID ?? 0,
                    AuditPhase = "",
                    UsersID = UsersID
                });
            }
        }

        public IActionResult OnPost()
        {
            UsersID = GetLoggedInUsersID();
            Console.WriteLine($"Inserting Timesheet for UsersID: {UsersID}");

            var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            LoadTimeSheetAuditPhaseOptions();

            for (int i = 0; i < 7; i++) // Loop for 7 days
            {
                string saveDateKey = $"Timesheets[{i}].Date";
                string hoursKey = $"Timesheets[{i}].Hours";
                string activityKey = $"Timesheets[{i}].Activity";
                string auditPhaseKey = $"Timesheets[{i}].AuditPhase";
                string auditIDKey = $"Timesheets[{i}].AuditID";

                if (!formData.TryGetValue(saveDateKey, out var dateValue) ||
                    !DateOnly.TryParse(dateValue, out var saveDate))
                {
                    ModelState.AddModelError("", $"Invalid date for Timesheets[{i}].Date.");
                    return Page();
                }

                if (!formData.TryGetValue(hoursKey, out var hours) || string.IsNullOrEmpty(hours))
                {
                    ModelState.AddModelError("", $"Please enter valid hours for {saveDate:yyyy-MM-dd}.");
                    return Page();
                }

                var activityValue = formData.TryGetValue(activityKey, out var activity) ? activity : null;
                var auditPhaseValue = formData.TryGetValue(auditPhaseKey, out var auditPhase) ? auditPhase : null;

                bool isAudit = formData.TryGetValue(auditIDKey, out var auditIDStr) && !string.IsNullOrEmpty(auditIDStr);
                var auditIDValue = isAudit ? (int?)int.Parse(auditIDStr) : null;

                var updatedEntry = new Timesheet
                {
                    Date = saveDate,
                    Hours = hours,
                    Activity = activityValue, 
                    AuditPhase = auditPhaseValue,
                    IsAudit = isAudit,
                    AuditID = auditIDValue,
                    UsersID = UsersID
                };

                string query = @"
INSERT INTO Timesheet (Date, Hours, Activity, AuditPhase, IsAudit, AuditID, UsersID)
VALUES (@Date, @Hours, @Activity, @AuditPhase, @IsAudit, @AuditID, @UsersID)";

                var parameters = new[]
                {
            new SqlParameter("@Date", updatedEntry.Date.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@Hours", updatedEntry.Hours),
            new SqlParameter("@Activity", updatedEntry.Activity ?? (object)DBNull.Value),
            new SqlParameter("@AuditPhase", updatedEntry.AuditPhase ?? (object)DBNull.Value),
            new SqlParameter("@IsAudit", updatedEntry.IsAudit),
            new SqlParameter("@AuditID", (object)updatedEntry.AuditID ?? DBNull.Value),
            new SqlParameter("@UsersID", updatedEntry.UsersID)
        };

                try
                {
                    DataCon.ExecNonQuery(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving timesheet entry for {saveDate:yyyy-MM-dd}: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the timesheet entry.");
                    return Page();
                }
            }

            return RedirectToPage("/Success", new { message = "Timesheet saved successfully!" });
        }


        private int GetLoggedInUsersID()
        {
            return HttpContext.Session.GetInt32("UsersID") ?? throw new InvalidOperationException("User is not logged in.");
        }




        public JsonResult OnGetLoadAudits()
        {
            string query = "SELECT AuditID AS id, AuditTitle AS activity FROM Audit";

            try
            {
                DataSet ds = DataCon.BuildDataSet(query);

                var audits = ds.Tables[0].Rows.Cast<DataRow>().Select(row => new
                {
                    id = row["id"],
                    activity = row["activity"]
                }).ToList();

                Console.WriteLine($"LoadAudits - {audits.Count} items loaded successfully.");
                return new JsonResult(audits);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading audits: {ex.Message}");
                return new JsonResult(new { error = "Error loading audits", message = ex.Message });
            }
        }

        public JsonResult OnGetLoadActivities()
        {
            string query = "SELECT TimeSheetActivityID AS id, Activity FROM TimeSheetActivity";

            try
            {
                DataSet ds = DataCon.BuildDataSet(query);

                var activities = ds.Tables[0].Rows.Cast<DataRow>().Select(row => new
                {
                    id = row["id"],
                    activity = row["activity"]
                }).ToList();

                Console.WriteLine($"LoadActivities - {activities.Count} items loaded successfully.");
                return new JsonResult(activities);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading activities: {ex.Message}");
                return new JsonResult(new { error = "Error loading activities", message = ex.Message });
            }
        }

        private void LoadTimeSheetAuditPhaseOptions()
        {
            TimeSheetAuditPhaseOptions = new List<SelectListItem>();
            string query = "SELECT TimeSheetAuditPhaseID, AuditPhase FROM TimeSheetAuditPhase";

            try
            {
                DataSet ds = DataCon.BuildDataSet(query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        TimeSheetAuditPhaseOptions.Add(new SelectListItem
                        {
                            Value = row["TimeSheetAuditPhaseID"].ToString(),
                            Text = row["AuditPhase"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading timesheet audit phase options: {ex.Message}");
            }
        }
    }
}
