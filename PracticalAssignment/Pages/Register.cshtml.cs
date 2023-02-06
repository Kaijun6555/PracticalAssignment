using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PracticalAssignment.Models;
using PracticalAssignment.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PracticalAssignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<FFMUser> userManager;
        private SignInManager<FFMUser> signInManager;
        private IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RoleManager<IdentityRole> roleManager;



        [BindProperty]
        public Register RModel { get; set; }

        //[FileExtensions(Extensions=".jpg",ErrorMessage ="JPG only")]
        [Required,BindProperty]
        public IFormFile Upload { get; set; }

        public RegisterModel(UserManager<FFMUser> userManager,
        SignInManager<FFMUser> signInManager,IWebHostEnvironment environment, IHttpContextAccessor contextAccessor, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._environment = environment;
            _contextAccessor = contextAccessor;
            this.roleManager = roleManager;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (ModelState.IsValid)
            {

                //Create Role if the role doesnt exist
                IdentityRole resultrole= await roleManager.FindByIdAsync("Admin");
                IdentityRole resultroleuser = await roleManager.FindByIdAsync("Standard");
                if (resultrole == null)
                {
                    IdentityResult roleresult2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!roleresult2.Succeeded)
                    {
                        Console.WriteLine("creation of role error");
                    }
                }
                if (resultroleuser == null)
                {
                    IdentityResult roleresultuser2 = await roleManager.CreateAsync(new IdentityRole("Standard"));
                    if (!roleresultuser2.Succeeded)
                    {
                        Console.WriteLine("creation of role standard error");
                    }
                }
                //JPG file validation
                if (Path.GetExtension(Upload.FileName) != ".jpg")
                {
                    ModelState.AddModelError("", "Only accepting JPG files");
                    return Page();
                }
                var imageRoute = "";
                //Image
                if (Upload != null)
                {
                    var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot\\uploads", imageFile);
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    imageRoute = "/uploads/" + imageFile;
                    RModel.ImageUrl = imageRoute;
                }

                //Generate UUID for key
                var encryptionkey = Guid.NewGuid().ToString();

                //Encrypt Data
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");

                var protector = dataProtectionProvider.CreateProtector(encryptionkey);

                var user = new FFMUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    CreditCard = protector.Protect(RModel.CreditCard),
                    DeliveryAddress = RModel.DeliveryAddress,
                    AboutMe = HttpUtility.HtmlEncode(RModel.AboutMe),
                    PhoneNumber = "+65" + RModel.PhoneNumber,
                    FullName = RModel.FullName,
                    Gender = RModel.Gender,
                    ImageUrl = imageRoute,
                    keyenc = encryptionkey
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Standard");
                    await signInManager.SignInAsync(user, false);
                    //Create the security context
                    var claims = new List<Claim> { };
                    var i = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                    //Session
                    _contextAccessor.HttpContext.Session.SetString("Email", RModel.Email);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
