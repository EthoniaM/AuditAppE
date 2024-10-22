using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KanBan.Pages
{
    public class SuccessModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet(string message)
        {
            Message = message;
        }
    }
}
