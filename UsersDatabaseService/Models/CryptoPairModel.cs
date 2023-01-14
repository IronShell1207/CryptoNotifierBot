using System.ComponentModel.DataAnnotations.Schema;

namespace UsersDatabaseService.Models
{
    public class CryptoPairModel
    {
        #region Public Properties

        public string CurrencyName { get; set; }
        public string CurrencyQuote { get; set; }
        public string ExchangePlatform { get; set; }
        public int Id { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public UserModel Owner { get; set; }

        public int OwnerId { get; set; }

        #endregion Public Properties
    }
}