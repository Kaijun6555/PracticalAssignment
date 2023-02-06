using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticalAssignment.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Web;

namespace PracticalAssignment.Pages
{
    public class IndexModel : PageModel

    {

        private readonly SignInManager<FFMUser> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<FFMUser> userManager;

        public IndexModel(IHttpContextAccessor contextAccessor, SignInManager<FFMUser> signInManager,UserManager<FFMUser> usermanager)

        {
            userManager = usermanager;
            _contextAccessor = contextAccessor;
            _signInManager = signInManager;
        }

        public string Email { get; set; }
        public FFMUser userObj { get; set; }
        public string creditCard { get; set; }

        public string DecodeString(string val)
        {
            var value = HttpUtility.HtmlDecode(val);
            return value;
        }
        public async void OnGet()
        {

            ////Google Retrieval
            //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            //{
            //    claim.Value,
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type
            //});
            //Console.WriteLine(claims);

            Email = _contextAccessor.HttpContext.Session.GetString("Email");
            if (Email != null)
            {
                Email = _contextAccessor.HttpContext.Session.GetString("Email");
                userObj = userManager.FindByEmailAsync(Email).Result;           
                userObj.AboutMe = DecodeString(userObj.AboutMe);
                //Decrypt Data
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");

                var protector = dataProtectionProvider.CreateProtector(userObj.keyenc);
                creditCard = protector.Unprotect(userObj.CreditCard);
            }
            else
            {   
                _contextAccessor.HttpContext.Session.Clear();
            }
        }
        
    }
}