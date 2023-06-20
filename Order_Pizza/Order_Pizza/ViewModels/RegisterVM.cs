using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Order_Pizza.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(250)]
        [Remote("IsAnyUserName", "Account", HttpMethod = "Post",AdditionalFields ="__RequestVerificationToken")]
        [Display(Name = "User name")]
        public string User_Name { get; set; }
        [Required]
        [EmailAddress]
        [Remote("IsAnyEmail", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string Email { get; set; }
        [Required]
        [Phone]
        [MaxLength(11)]
        public string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RePassword { get; set; }
    }
}
