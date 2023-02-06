using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PracticalAssignment.ViewModels
{
    public class Register
    {
        [Required]
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Only letters allowed for Name Field ")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"[0-9]*", ErrorMessage = "Only numerics allowed for phone number ")] 
        [MinLength(8,ErrorMessage ="Please enter a valid phone number format")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [RegularExpression(@"[0-9]*", ErrorMessage = "Enter a valid credit card number (16 digits)")]
        [MinLength(16, ErrorMessage = "Enter a valid credit card number (16 digits)")]
        [MaxLength(16, ErrorMessage = "Enter a valid credit card number (16 digits)")]
        public string CreditCard { get; set; }

        [DataType(DataType.Text)]
        public string DeliveryAddress { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string ImageUrl = "/upload/man.png";


        [DataType(DataType.Text)]
        public string AboutMe { get; set; }

        [DataType(DataType.Password)]
        [MinLength(12,ErrorMessage ="Enter a minimum of 12 characters for your password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
