using Microsoft.Build.Framework;
using Order_Pizza.Models;
using System.ComponentModel;

namespace Order_Pizza.ViewModels
{
    public class PizzaVM
    {
        public Pizza Pizza { get; set; }
        [DisplayName("Pizza Image")]
        public IFormFile? Image_Pizza { get; set; }
    }
}
