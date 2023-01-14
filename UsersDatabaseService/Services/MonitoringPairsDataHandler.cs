using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using UsersDatabaseService.Models;

namespace UsersDatabaseService.Services
{
    public class MonitoringPairsDataHandler
    {
        /// <summary>
        /// Получает юзера по ид.
        /// </summary>
        public async Task AddNewPair(UserModel user, MonitoringPair pair)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                UserModel userFromDb = await dbContext.Users.Include(x => x.MonitoringPairs).FirstAsync(x => x == user);
                if (user != null)
                {
                    userFromDb.MonitoringPairs.Add(pair);
                }
                else
                {
                    throw new SqlNullValueException("User not found!");
                }
            }
        }

        /// <summary>
        /// Получает все пары из бд.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        /// <returns>Пары пользователя.</returns>
        /// <exception cref="SqlNullValueException">Если пользователь не найден.</exception>
        public async Task<List<MonitoringPair>> GetAllUserPairs(UserModel user)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                UserModel userFromDb = await dbContext.Users.Include(x => x.MonitoringPairs).FirstAsync(x => x == user);
                if (user != null)
                {
                    return userFromDb.MonitoringPairs;
                }
                else
                {
                    throw new SqlNullValueException("User not found!");
                }
            }
        }

        /// <summary>
        /// Обновляет данные.
        /// </summary>
        /// <param name="pair">Пара.</param>
        public async Task ModifyNewPair(MonitoringPair pair)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                MonitoringPair? pairFromDb = await dbContext.MonitoringPairs.FirstOrDefaultAsync(x => x.Id == pair.Id);
                if (pairFromDb != null)
                {
                    dbContext.MonitoringPairs.Entry(pairFromDb).CurrentValues.SetValues(pair);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}