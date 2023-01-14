using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using UsersDatabaseService.Models;

namespace UsersDatabaseService.Services
{
    public class UserDataRequestHandler
    {
        #region Public Methods

        /// <summary>
        /// Создает пользователя по ид.
        /// </summary>
        public async Task<UserModel> CreateUserByTelegramId(long telegramId)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                var newUser = new UserModel()
                {
                    TelegramId = telegramId
                };
                await dbContext.Users.AddAsync(newUser);
                await dbContext.SaveChangesAsync();
                return newUser;
            }
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                return await dbContext.Users.Include(x => x.TelegramSettings).ToListAsync();
            }
        }

        /// <summary>
        /// Получает юзера по ид.
        /// </summary>
        public async Task<UserModel?> GetUserById(int id)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                return await dbContext.Users
                    .Include(x => x.TelegramSettings)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        /// <summary>
        /// Обновляет данные пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="SqlNullValueException"></exception>
        public async Task ModifyUser(UserModel user)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                UserModel? userFromDb = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (userFromDb != null)
                {
                    dbContext.Users.Entry(userFromDb).CurrentValues.SetValues(user);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new SqlNullValueException("User not found!");
                }
            }
        }

        /// <summary>
        /// Получает юзера по телеграм ид.
        /// </summary>
        public async Task<UserModel?> GetUserByTelegramId(long telegramId)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                return await dbContext.Users
                    .Include(x => x.TelegramSettings)
                    .FirstOrDefaultAsync(x => x.TelegramId == telegramId);
            }
        }

        public async Task<UserModel?> GetUserPairsById(int id)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                return await dbContext.Users
                    .Include(x => x.TelegramSettings)
                    .Include(x => x.MonitoringPairs)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<UserModel?> GetUserPairsByTelegramId(long telegramId)
        {
            using (UsersDatabaseContext dbContext = new UsersDatabaseContext())
            {
                return await dbContext.Users
                    .Include(x => x.TelegramSettings)
                    .Include(x => x.MonitoringPairs)
                    .FirstOrDefaultAsync(x => x.TelegramId == telegramId);
            }
        }

        public async Task<UserModel?> GetUserPairsByUser(UserModel user) => await GetUserPairsById(user.Id);

        #endregion Public Methods
    }
}