using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticalAssignment.Models;

namespace PracticalAssignment.Pages
{
    public class LogoutModel : PageModel
    {

        private readonly IHttpContextAccessor contxt;

        private readonly SignInManager<FFMUser> signInManager;
        public LogoutModel(SignInManager<FFMUser> signInManager, IHttpContextAccessor contxt)
        {
            this.signInManager = signInManager;
            this.contxt = contxt;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            contxt.HttpContext.Session.Clear();
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
