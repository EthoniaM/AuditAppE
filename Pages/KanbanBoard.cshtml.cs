using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace KanBan.Pages
{
    public class KanbanBoardModel : PageModel
    {
        public int? AuditID { get; set; }
        public IList<Kanban> KanbanTasks { get; set; } = new List<Kanban>();
        public Audit CurrentAudit { get; set; }

        public string GetPriorityClass(string? priority)
        {
            return priority switch
            {
                "High" => "badge-high",
                "Medium" => "badge-medium",
                "Low" => "badge-low",
                _ => "badge-secondary"
            };
        }

        public async Task<IActionResult> OnGetAsync(int? auditId)
        {
            AuditID = auditId;

            if (auditId.HasValue)
            {
                var auditQuery = "SELECT * FROM Audit WHERE AuditID = @AuditID";
                var auditParameters = new SqlParameter("@AuditID", auditId.Value);

                var auditDataSet = DataCon.BuildDataSet(auditQuery, new[] { auditParameters });
                if (auditDataSet.Tables[0].Rows.Count > 0)
                {
                    var row = auditDataSet.Tables[0].Rows[0];
                    CurrentAudit = new Audit
                    {
                        AuditID = Convert.ToInt32(row["AuditID"]),
                        AuditTitle = row["AuditTitle"].ToString()
                    };

                    // Debugging output
                    Console.WriteLine($"Audit found: {CurrentAudit.AuditTitle}");
                }
                else
                {
                    // Debugging output
                    Console.WriteLine("Audit not found.");
                    return NotFound();
                }

                var kanbanQuery = "SELECT * FROM Kanban WHERE AuditID = @AuditID";
                var kanbanParameters = new SqlParameter("@AuditID", auditId.Value);

                var kanbanDataSet = DataCon.BuildDataSet(kanbanQuery, new[] { kanbanParameters });

                KanbanTasks = kanbanDataSet.Tables[0].AsEnumerable().Select(row => new Kanban
                {
                    KanBanID = Convert.ToInt32(row["KanbanID"]),
                    AuditID = !Convert.IsDBNull(row["AuditID"]) ? (int?)Convert.ToInt32(row["AuditID"]) : null,
                    TaskTitle = row["TaskTitle"].ToString(),
                    Status = row["Status"].ToString(),
                    Priority = row["Priority"].ToString(),
                    TaskDescription = row["TaskDescription"].ToString(),
                    StartDate = !Convert.IsDBNull(row["StartDate"]) ? (DateTime?)Convert.ToDateTime(row["StartDate"]) : null,
                    EndDate = !Convert.IsDBNull(row["EndDate"]) ? (DateTime?)Convert.ToDateTime(row["EndDate"]) : null
                }).ToList();

                // Debugging output
                Console.WriteLine($"Retrieved {KanbanTasks.Count} tasks.");
                await UpdateTimesAsync(KanbanTasks);
            }
            else
            {
                CurrentAudit = null;
                var kanbanQuery = "SELECT * FROM Kanban";
                var kanbanDataSet = DataCon.BuildDataSet(kanbanQuery);

                KanbanTasks = kanbanDataSet.Tables[0].AsEnumerable().Select(row => new Kanban
                {
                    KanBanID = Convert.ToInt32(row["KanbanID"]),
                    AuditID = !Convert.IsDBNull(row["AuditID"]) ? (int?)Convert.ToInt32(row["AuditID"]) : null,
                    TaskTitle = row["TaskTitle"].ToString(),
                    Status = row["Status"].ToString(),
                    Priority = row["Priority"].ToString(),
                    TaskDescription = row["TaskDescription"].ToString(),
                    StartDate = !Convert.IsDBNull(row["StartDate"]) ? (DateTime?)Convert.ToDateTime(row["StartDate"]) : null,
                    EndDate = !Convert.IsDBNull(row["EndDate"]) ? (DateTime?)Convert.ToDateTime(row["EndDate"]) : null
                }).ToList();

                // Debugging output
                Console.WriteLine($"Retrieved {KanbanTasks.Count} tasks.");
            }

            return Page();
        }

        private async Task UpdateTimesAsync(IList<Kanban> tasks)
        {
            var currentTime = DateTime.Now.TimeOfDay;

            foreach (var task in tasks)
            {
                if (task.StartDate.HasValue)
                {
                    task.StartDate = task.StartDate.Value.Date.Add(currentTime);
                }
                if (task.EndDate.HasValue)
                {
                    task.EndDate = task.EndDate.Value.Date.Add(currentTime);
                }
            }

            // Save changes using DataCon
            foreach (var task in tasks)
            {
                var updateQuery = "UPDATE Kanban SET StartDate = @StartDate, EndDate = @EndDate WHERE KanbanID = @KanbanID";
                var parameters = new[]
                {
                    new SqlParameter("@StartDate", (object)task.StartDate ?? DBNull.Value),
                    new SqlParameter("@EndDate", (object)task.EndDate ?? DBNull.Value),
                    new SqlParameter("@KanbanID", task.KanBanID)
                };

                DataCon.ExecNonQuery(updateQuery, parameters);
            }
        }

        public async Task<IActionResult> OnPostCreateTaskAsync(Kanban newTask)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var insertQuery = "INSERT INTO Kanban (AuditID, TaskTitle, Status, Priority, TaskDescription, StartDate, EndDate) VALUES (@AuditID, @TaskTitle, @Status, @Priority, @TaskDescription, @StartDate, @EndDate)";
            var parameters = new[]
            {
                new SqlParameter("@AuditID", newTask.AuditID ?? (object)DBNull.Value),
                new SqlParameter("@TaskTitle", newTask.TaskTitle),
                new SqlParameter("@Status", newTask.Status),
                new SqlParameter("@Priority", newTask.Priority),
                new SqlParameter("@TaskDescription", newTask.TaskDescription),
                new SqlParameter("@StartDate", (object)newTask.StartDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object)newTask.EndDate ?? DBNull.Value)
            };

            DataCon.ExecNonQuery(insertQuery, parameters);

            return RedirectToPage();
        }

        public class UpdateStatusRequest
        {
            public int Id { get; set; }
            public string? Status { get; set; }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            try
            {
                // Read and send request body
                var jsonData = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<UpdateStatusRequest>(jsonData);

                if (data == null)
                {
                    return BadRequest("Invalid data.");
                }

                var updateQuery = "UPDATE Kanban SET Status = @Status WHERE KanbanID = @KanbanID";
                var parameters = new[]
                {
                    new SqlParameter("@Status", data.Status ?? (object)DBNull.Value),
                    new SqlParameter("@KanbanID", data.Id)
                };

                DataCon.ExecNonQuery(updateQuery, parameters);

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (you might use a logging framework like Serilog or NLog)
                // Log.Error(ex, "An error occurred while updating task status.");

                // Return a server error response
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
