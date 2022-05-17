using System.ComponentModel.DataAnnotations;

namespace WebApiPortal.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Telegram Id required")]
        public string? TelegramId { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
