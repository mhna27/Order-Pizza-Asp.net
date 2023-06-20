using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Order_Pizza.ViewModels
{
    public class ResetPasswordVM
    {
        [Display(Name = "New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name = "Confirm New Password")]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
        [Required]
        public string User_Id { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
