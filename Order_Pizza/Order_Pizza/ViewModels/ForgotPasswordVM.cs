using System.ComponentModel.DataAnnotations;

namespace Order_Pizza.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [Phone]
        [MaxLength(11)]
        public string Phone { get; set; }
    }
}
