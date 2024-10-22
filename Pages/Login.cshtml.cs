using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            string query = "SELECT UsersID, FirstName FROM Users WHERE Email = @Email AND Password = @Password";

            var parameters = new[]
            {
                new SqlParameter("@Email", Email),
                new SqlParameter("@Password", Password) // Ensure password is hashed in production
            };

            try
            {
                var dataset = DataCon.BuildDataSet(query, parameters);
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    int usersID = (int)dataset.Tables[0].Rows[0]["UsersID"];
                    string firstName = dataset.Tables[0].Rows[0]["FirstName"].ToString();

                    // Store UsersID and FirstName in session
                    HttpContext.Session.SetInt32("UsersID", usersID);
                    HttpContext.Session.SetString("FirstName", firstName);

                    return RedirectToPage("/Timesheet"); // Redirect to the timesheet page
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return Page();
            }
        }
    }
}
