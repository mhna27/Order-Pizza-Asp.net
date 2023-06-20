using System.ComponentModel.DataAnnotations;

namespace Order_Pizza.ViewModels
{
    public class ConfirmMobileVM
    {
        [Required, StringLength(6),Display(Name ="SMS Code")]
        public string Sms_Code { get; set; }

        public string Phone { get; set; }
        public string Code { get; set; }
        public string Redirect_To_Action { get; set; }
    }
}
