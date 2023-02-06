using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PracticalAssignment.Models;
using PracticalAssignment.Services;
using PracticalAssignment.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace PracticalAssignment.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpContextAccessor contxt;
        private readonly UserManager<FFMUser> userManager;

        [BindProperty]
        public Login LModel { get; set; }
        public FFMUser userObj { get; set; }
        public AuthMessageSender Messenger { get; set; } = new AuthMessageSender();

        private readonly SignInManager<FFMUser> signInManager;
        public LoginModel(SignInManager<FFMUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<FFMUser> usermanager)
        {
            this.signInManager = signInManager;
            this.contxt = httpContextAccessor;
            this.userManager = usermanager;
        }



        public class GoogleAccessToken

        {

            public string access_token { get; set; }

            public string token_type { get; set; }

            public int expires_in { get; set; }

            public string id_token { get; set; }

            public string refresh_token { get; set; }

        }

        public class GoogleUserOutputData

        {

            public string id { get; set; }

            public string name { get; set; }

            public string given_name { get; set; }

            public string email { get; set; }

            public string picture { get; set; }

        }


        public void GetUserInfo(string access_token)
        {
            try

            {

                HttpClient client = new HttpClient();

                var urlProfile = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + access_token;

                client.CancelPendingRequests();

                HttpResponseMessage output = client.GetAsync(urlProfile).Result;

                if (output.IsSuccessStatusCode)

                {

                    string outputData = output.Content.ReadAsStringAsync().Result;

                    var serStatus = JsonConvert.DeserializeObject<GoogleUserOutputData>(outputData);
                }
            }

            catch (Exception ex)

            {

                //catching the exception

            }

        }



        public void OnGet()
        {

    
        }


        public async Task<IActionResult> OnPostGoogleAsync()
        {
            var p = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(p, GoogleDefaults.AuthenticationScheme);

        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Value,
                claim.Type
            });
            Console.WriteLine(claims);
            return Page();
        }
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);
                if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "The account is locked out. Please reset your password with the email sent to your account");
                    return Page();
                }
                if (identityResult.Succeeded)
                {
                    //Create the security context
                    var claims = new List<Claim> { };
                    var i = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    //Retrieve User
                    userObj = userManager.FindByEmailAsync(LModel.Email).Result;

                    //Generate OTP
                    Random rnd = new Random();
                    StringBuilder bld = new StringBuilder();
                    for (int no = 0; no < 6; ++no)
                    {
                        int digitGen = rnd.Next(10);
                        bld.Append(digitGen);
                    }
                    string otpass = bld.ToString();
                    userObj.tempOTP = otpass;
                    await userManager.UpdateAsync(userObj);
                    await Messenger.SendSmsAsync(userObj.PhoneNumber, "Your OTP is" + otpass);
                    Console.WriteLine(otpass);
                    contxt.HttpContext.Session.SetString("Email", LModel.Email);
                    return RedirectToPage("VerifyOTP");
                }

                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();
        }
    }
}
