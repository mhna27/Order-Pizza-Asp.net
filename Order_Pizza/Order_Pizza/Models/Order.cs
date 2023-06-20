using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order_Pizza.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Pizza_Id { get; set; }

        [ForeignKey("Pizza_Id")]
        [ValidateNever]
        public Pizza Pizza { get; set; }

        [Required]
        [Display(Name = "Order number")]
        [Range(1, 20)]
        public int Quantity { get; set; }

        [Required]
        public DateTime Date_Time_Insert { get; set; } = DateTime.Now;
        [Required]
        public string User_Id { get; set; }

        [ForeignKey("User_Id")]
        [ValidateNever]
        public IdentityUser IdentityUser { get; set; }

    }
}
