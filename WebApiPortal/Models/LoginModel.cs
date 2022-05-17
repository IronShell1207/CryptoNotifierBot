using System.ComponentModel.DataAnnotations;

namespace WebApiPortal.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? TelegramId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
