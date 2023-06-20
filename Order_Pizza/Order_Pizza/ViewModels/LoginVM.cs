using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Order_Pizza.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MaxLength(250)]
        [Display(Name = "User name")]
        public string User_Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        [MaxLength(11)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me?")]
        public bool Remember_Me { get; set; }
    }
}

