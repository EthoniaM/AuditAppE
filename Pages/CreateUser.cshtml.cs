using KanBan.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public User User { get; set; } = new User();

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
            string sqlQuery = "INSERT INTO Users (FirstName, LastName, Email, Password,Role) VALUES (@FirstName, @LastName, @Email, @Password, @Role)";
            var parameters = new SqlParameter[]
            {
    new SqlParameter("@FirstName", User.FirstName),
    new SqlParameter("@LastName", User.LastName),
    new SqlParameter("@Email", User.Email),
    new SqlParameter("@Password", User.Password), // Remember to hash this in a real applicati
    new SqlParameter("@Role", User.Role) // Include Role in the parameters
            };

            // Execute the SQL command
            try
            {
                DataCon.ExecNonQuery(sqlQuery, parameters);
                return RedirectToPage("/Success", new { message = "Users Created successfully!" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the user: {ex.Message}");
                return Page();
            }
        }
    }
}
 