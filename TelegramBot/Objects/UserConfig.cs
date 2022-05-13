﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        public string? CryptoNotifyStyle { get; set; }
        public string Language { get; set; } = "en";
        public bool DisplayTaskEditButtonsInNotifications { get; set; } = true;
        public bool RemoveLatestNotifyBeforeNew { get; set; } = false;
        public bool NightModeEnable { get; set; } = false;
        /// <summary>
        /// Time in minutes (23*60 by default)
        /// </summary>
        public int NightModeStartTime { get; set; } = 1380;
        /// <summary>
        /// Time in minutes (6*60 by default)
        /// </summary>
        public int NightModeEndsTime { get; set; } = 180;
        /// <summary>
        /// Shows Pumps and Dumps info in notifications
        /// </summary>
        public bool ShowMarketEvents { get; set; } = true;
        /// <summary>
        /// Spreads notify of selected pairs of changes in 24h and 8h in selected time
        /// </summary>
        public int? MorningReport { get; set; } = null;

        public List<CryptoPair>? pairs { get; set; } = new();
    }
}