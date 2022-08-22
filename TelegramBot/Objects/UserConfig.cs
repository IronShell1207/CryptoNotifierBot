using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class UserConfig
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// Notifications delay in seconds
        /// </summary>
        public int NoticationsInterval { get; set; } = 300;

        /// <summary>
        /// Automaticaly amplify notification interval if notifications keep coming
        /// </summary>
        public bool AntifloodIntervalAmplification { get; set; } = true;

        /// <summary>
        /// Compact notifications without any notes for true value, and full info notifications for false
        /// </summary>
        public bool NotesEnable { get; set; } = false;

        /// <summary>
        /// Compact pairs notifications for true value, and full info for false
        /// </summary>
        public string CryptoNotifyStyle { get; set; } = "";

        public string Language { get; set; } = "en";

        /// <summary>
        /// If enabled, when user creates new tasks its trigger once by default
        /// </summary>
        public bool TriggerOneTasksByDefault { get; set; } = false;

        public bool SetExchangeAutomaticaly { get; set; } = false;

        /// <summary>
        /// Not uses and not created
        /// </summary>
        public bool DisplayTaskEditButtonsInNotifications { get; set; } = true;

        /// <summary>
        /// Removes last notification before sending a new one. (if enabled)
        /// </summary>
        public bool RemoveLatestNotifyBeforeNew { get; set; } = false;

        /// <summary>
        /// If night mode enabled and time setted correctly notifications will not be sended in that period of time.
        /// </summary>
        public bool NightModeEnable { get; set; } = false;

        /// <summary>
        /// Time in minutes (23*60 by default)
        /// </summary>
        public TimeSpan NightModeStartTime { get; set; } = TimeSpan.FromHours(23);

        /// <summary>
        /// Time in minutes (6*60 by default)
        /// </summary>
        public TimeSpan NightModeEndsTime { get; set; } = TimeSpan.FromHours(6);

        public int TimezoneChange { get; set; } = 3;

        /// <summary>
        /// Shows Pumps and Dumps info in notifications
        /// </summary>
        public bool ShowMarketEvents { get; set; } = true;

        /// <summary>
        /// Spreads notify of selected pairs of changes in 24h and 8h in selected time
        /// </summary>
        public int MorningReport { get; set; } = 0;

        public BreakoutSub? SubSets { get; set; }
        public List<CryptoPair>? pairs { get; set; } = new();
        public List<MessageAccepted>? Messages { get; set; } = new();

        /// <summary>
        /// Одинаковы ли конфигурации.
        /// </summary>
        public bool AreEqual(UserConfig cfg)
        {
            var props = this.GetType().GetProperties().Where(x => x.PropertyType.IsValueType);
            foreach (var prop in props)
            {
                var obj1 = prop.GetValue(this);
                var obj2 = prop.GetValue(cfg);
                if (!obj1.Equals(obj2))
                    return false;
            }
            return true;
        }

        public string GetUserSettingsString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"User telegram id: {TelegramId}");
            builder.Append($"User id in db: {Id}");
            builder.Append($"Notification interval: {NoticationsInterval} secs.");
            builder.Append($"Automaticaly set exchange for new pairs: {SetExchangeAutomaticaly}");
            builder.Append($"Night mode: {NightModeEnable}. Start: {NightModeStartTime}, End: {NightModeEndsTime}");
            return builder.ToString();
        }
    }
}