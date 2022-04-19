using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Objects;

namespace TelegramBot.Static.DbOperations
{
    public class CryptoPairDbHandler : IMyDisposable
    {
        public bool DeletePair(CryptoPair pair) => DeletePair(pair.Id, pair.OwnerId);
        public bool DeletePair(int id, int ownerId)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pair = dbContext.CryptoPairs.FirstOrDefault(x => x.Id == id && x.OwnerId == ownerId);
                if (pair != null)
                {
                    dbContext.CryptoPairs.Remove(pair);
                    return true;
                }

                return false;
            }
        }

        public CryptoPair GetPairFromId(int id, int ownerId)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pair = dbContext.CryptoPairs.FirstOrDefault(x => x.Id == id && x.OwnerId == ownerId);
                if (pair != null)
                {
                    return pair;
                }
                return null;
            }
        }
    }
}
