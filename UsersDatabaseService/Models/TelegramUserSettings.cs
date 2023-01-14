using System.ComponentModel.DataAnnotations.Schema;

namespace UsersDatabaseService.Models
{
    /// <summary>
    /// Настройки пользователя телеграм.
    /// </summary>
    public sealed class TelegramUserSettings
    {
        #region Public Properties

        /// <summary>
        /// Automaticaly amplify notification interval if notifications keep coming
        /// </summary>
        public bool AntiFloodIntervalAmplification { get; set; } = true;

        public int Id { get; set; }
        public string Language { get; set; } = "en";
        public bool MorningReport { get; set; } = false;

        /// <summary>
        /// If night mode enabled and time setted correctly notifications will not be sended in that period of time.
        /// </summary>
        public bool NightModeEnable { get; set; } = false;

        /// <summary>
        /// Time in minutes (6*60 by default)
        /// </summary>
        public TimeSpan NightModeEndsTime { get; set; } = TimeSpan.FromHours(6);

        /// <summary>
        /// Time in minutes (23*60 by default)
        /// </summary>
        public TimeSpan NightModeStartTime { get; set; } = TimeSpan.FromHours(23);

        /// <summary>
        /// Notifications delay in seconds
        /// </summary>
        public int NoticationsInterval { get; set; } = 300;

        /// <summary>
        /// Автоматически выбирает первую подходящюю биржу
        /// </summary>
        public bool AutoSetExchange { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        public int Timezone { get; set; } = 7;

        /// <summary>
        /// If enabled, when user creates new tasks its trigger once by default
        /// </summary>
        public bool ShowChangesOnce { get; set; } = false;

        public UserModel? User { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        #endregion Public Properties
    }
}