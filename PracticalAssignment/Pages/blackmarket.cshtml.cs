using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PracticalAssignment.Pages
{
    [Authorize(Roles ="Admin")]
    public class blackmarketModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
