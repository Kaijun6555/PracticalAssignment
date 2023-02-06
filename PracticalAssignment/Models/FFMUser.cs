using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PracticalAssignment.Models
{
	public class FFMUser : IdentityUser
	{
        public string FullName { get; set; }
        public string CreditCard { get; set; }
        public string ImageUrl { get; set; }
        public string AboutMe { get; set; }
        public string DeliveryAddress { get; set; }
        public string Gender { get; set; }
        public string keyenc { get; set; }
        public string tempOTP { get; set; } = "";
    }
}
