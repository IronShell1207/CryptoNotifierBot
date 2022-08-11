using System.ComponentModel.DataAnnotations;

namespace WebApiPortal.Models
{
    public class RegisterAdminModel : RegisterModel
    {
        [Required(ErrorMessage = "Telegram Id required")]
        public string PrivateKey { get; set; }
    }
}
