using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PracticalAssignment.Pages.Profile
{
    public class profileModel : PageModel
    {

        private readonly IHttpContextAccessor _contextAccessor;

        public profileModel(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Email;
        public void OnGet()
        {
            Email = _contextAccessor.HttpContext.Session.GetString("Email");
        }
    }
}
