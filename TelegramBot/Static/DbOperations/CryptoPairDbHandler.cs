using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                    dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> CompletlyEditCryptoPair(CryptoPair pair)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pairEditable = dbContext.CryptoPairs.OrderBy(x => x.Id).FirstOrDefault(x => x.Id == pair.Id);
                if (pairEditable != null)
                {
                    pairEditable.PairQuote = pair.PairQuote;
                    pairEditable.Price = pair.Price;
                    pairEditable.Triggered = pair.Triggered;
                    pairEditable.TriggerOnce = pair.TriggerOnce;
                    pairEditable.Enabled = pair.Enabled;
                    pairEditable.ExchangePlatform = pair.ExchangePlatform;
                    pairEditable.GainOrFall = pair.GainOrFall;
                    pairEditable.PairBase = pair.PairBase;
                    pairEditable.Note = pair.Note;
                    _ = dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> SetNewPriceFromPair(CryptoPair pair, double price) =>
            SetNewPriceTriggerPair(pair.Id, pair.OwnerId, price).Result;
        public async Task<bool> SetNewPriceTriggerPair(int id, int ownerId, double price)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pair = dbContext.CryptoPairs.FirstOrDefault(x => x.Id == id && x.OwnerId == ownerId);
                if (pair != null)
                {
                    pair.Price = price;
                    _ = dbContext.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public CryptoPair GetPairFromId(int id, int ownerId)
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                var pair = dbContext.CryptoPairs.Include(x=>x.User).FirstOrDefault(x => x.Id == id && x.User.Id == ownerId);
                if (pair != null)
                {
                    return pair;
                }
                return null;
            }
        }
    }
}
