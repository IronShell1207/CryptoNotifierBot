using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Constants;

namespace TelegramBot.Static.MessageHandlers
{
    public class UserSettingsMsgHandler : IDisposable
    {
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (disposing)
                { }
                disposed = true;
            }
        }

        public async void SetTimeZone(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = await BotApi.GetUserSettings(update, dbContext);
                var match = CommandsRegex.SetTimeZoneCommandRegex.Match(update.Message.Text);
                var timez = int.Parse(match.Groups["time"].Value);
                user.TimezoneChange = timez;
                dbContext.SaveChangesAsync();
                await BotApi.SendMessage(user.TelegramId, $"Time zone setted to {timez}");
            }
        }

        public async void SetNightTime(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var match = CommandsRegex.SettingsCommands.SetNightTime.Match(update.Message?.Text);
                if (match.Success)
                {
                    TimeSpan timeStart = TimeSpan.Parse(match.Groups["timestart"].Value);
                    TimeSpan timeEnd = TimeSpan.Parse(match.Groups["timeend"].Value);
                    var userConfig = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                    userConfig.NightModeStartTime = timeStart;
                    userConfig.NightModeEndsTime = timeEnd;
                    dbContext.SaveChangesAsync();
                    await BotApi.SendMessage(userConfig.TelegramId,
                        $"Night time set to: start - {timeStart}, end - {timeEnd}, enable - {userConfig.NightModeEnable}");
                }
            }
        }
                
        public async void SetEnableNightMode(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var userConfig = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                userConfig.NightModeEnable = !userConfig.NightModeEnable;
                dbContext.SaveChangesAsync();
                await BotApi.SendMessage(userConfig.TelegramId,
                    $"Night time enable set to - {userConfig.NightModeEnable}");
            }
        }
    }
}
