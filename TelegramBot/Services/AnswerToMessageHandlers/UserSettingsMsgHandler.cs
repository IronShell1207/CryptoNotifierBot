using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using Telegram.Bot.Types;
using TelegramBot.Constants.Commands;
using TelegramBot.Helpers;

namespace TelegramBot.Static.MessageHandlers
{
    public class UserSettingsMsgHandler
    {
        /*private bool disposed = false;

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

        public async Task SetTimeZone(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var user = await BotApi.GetUserSettings(update, dbContext);
                var match = SettingsCommands.SetTimeZoneCommandRegex.Match(update.Message.Text);
                var timez = int.Parse(match.Groups["time"].Value);
                user.TimezoneChange = timez;
                await dbContext.SaveChangesAsync();
                await BotApi.SendMessage(user.TelegramId, $"Time zone setted to {timez}");
            }
        }

        public async Task SetNightTime(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var match = SettingsCommands.SetNightTime.Match(update.Message?.Text);
                if (match.Success)
                {
                    TimeSpan timeStart = TimeSpan.Parse(match.Groups["timestart"].Value);
                    TimeSpan timeEnd = TimeSpan.Parse(match.Groups["timeend"].Value);
                    var userConfig = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                    userConfig.NightModeStartTime = timeStart;
                    userConfig.NightModeEndsTime = timeEnd;
                    await dbContext.SaveChangesAsync();
                    await BotApi.SendMessage(userConfig.TelegramId,
                        $"Night time set to: start - {timeStart}, end - {timeEnd}, enable - {userConfig.NightModeEnable}");
                }
            }
        }

        public async Task SetEnableNightMode(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var userConfig = dbContext.Users.First(x => x.TelegramId == update.Message.From.Id);
                userConfig.NightModeEnable = !userConfig.NightModeEnable;
                await dbContext.SaveChangesAsync();
                await BotApi.SendMessage(userConfig.TelegramId,
                    $"Night time enable set to - {userConfig.NightModeEnable}");
            }
        }

        public async Task ShowSettings(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var userConfig = dbContext.Users.FirstOrDefault(x =>
                    x.TelegramId == TelegramUpdatesHelper.GetTelegramIdFromUpdate(update));
                if (userConfig != null)
                {
                    await BotApi.SendMessage(userConfig.TelegramId, userConfig.GetUserSettingsString());
                }
            }
        }

        public async Task TurnLastMsgCleaning(Update update)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var sk = TelegramUpdatesHelper.GetTelegramIdFromUpdate(update);
                var userCfg = dbContext.Users.FirstOrDefault(x =>
                    x.TelegramId == sk.Identifier);
                if (userCfg != null)
                {
                    userCfg.RemoveLatestNotifyBeforeNew = true;
                    await dbContext.SaveChangesAsync();
                    await BotApi.SendMessage(userCfg.TelegramId, $"Last message cleaning status: {userCfg.RemoveLatestNotifyBeforeNew}");
                }
            }
        }*/
    }
}