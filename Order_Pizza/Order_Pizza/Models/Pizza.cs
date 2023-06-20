using MessagePack;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Order_Pizza.Models
{
    public class Pizza
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        [DisplayName("Ingredients")]
        public string Description { get; set; }
        [Required]
        [ValidateNever]
        [DisplayName("Image")]
        public string Image_Url { get; set; }
    }
}
