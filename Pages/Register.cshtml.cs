using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KanBan.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnPost()
        {
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return Page();
            }

            string query = @"
            INSERT INTO Users (FirstName, LastName, Email, Password)
            VALUES (@FirstName, @LastName, @Email, @Password)";

            var parameters = new[]
            {
            new SqlParameter("@FirstName", FirstName),
            new SqlParameter("@LastName", LastName),
            new SqlParameter("@Email", Email),
            new SqlParameter("@Password", Password) // Remember to hash the password
        };

            try
            {
                DataCon.ExecNonQuery(query, parameters);
                return RedirectToPage("/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return Page();
            }
        }
    }

}

