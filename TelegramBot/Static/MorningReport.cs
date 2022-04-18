using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TelegramBot.Objects;

namespace TelegramBot.Static
{
    public class MorningReport
    {
        public static void MorningSpread(AppDbContext dbContext)
        {
            var users = dbContext.Users.Where(x=>x.MorningReport != null).ToList();
            foreach (var user in users)
            {
                var timingHours = Math.Round((decimal) user.MorningReport / 60);

                var timingMins = user.MorningReport;
            }
        }
    }
}
