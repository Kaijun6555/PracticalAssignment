using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PracticalAssignment.Models;
using System.ComponentModel.DataAnnotations;

namespace PracticalAssignment.Pages
{
    public class VerifyOTPModel : PageModel
    {

        private readonly IHttpContextAccessor _contxt;
        private readonly UserManager<FFMUser> userManager;


        public VerifyOTPModel(IHttpContextAccessor contxt,UserManager<FFMUser> _usermng)
        {
            this._contxt = contxt;
            this.userManager = _usermng;
        }

        [StringLength(6,MinimumLength =6,ErrorMessage ="Please Enter a Valid Otp")]
        [BindProperty]
        public string OTP { get; set; }

        public FFMUser userobj { get; set; }
        public int wrongEntry { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {   
            if (ModelState.IsValid)
            {   
                if(wrongEntry >= 5)
                {
                    _contxt.HttpContext.Session.Clear();
                    return RedirectToPage("Index");
                }
                var sessionEmail = _contxt.HttpContext.Session.GetString("Email");
                userobj = userManager.FindByEmailAsync(sessionEmail).Result;
                var userotp = userobj.tempOTP;
                if (OTP == userotp)
                {
                    userobj.tempOTP = "";
                    await userManager.UpdateAsync(userobj);
                    return RedirectToPage("Index");
                }
                ModelState.AddModelError("", "Invalid OTP");
                wrongEntry += 1;
                return Page();
            }
            return Page();
        }
    }
}
