using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersDatabaseService.Models
{
    /// <summary>
    /// Модель пользователя.
    /// </summary>
    public class UserModel
    {
        #region Public Properties

        [Column("id")]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(100)]
        public long? TelegramId { get; set; }

        public TelegramUserSettings TelegramSettings { get; set; } = new TelegramUserSettings();

        public List<MonitoringPair> MonitoringPairs { get; set; } = new List<MonitoringPair>();

        #endregion Public Properties
    }
}