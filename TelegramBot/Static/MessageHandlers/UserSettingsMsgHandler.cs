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
                BotApi.SendMessage(user.TelegramId, $"Time zone setted to {timez}");
            }
        }
    }
}
