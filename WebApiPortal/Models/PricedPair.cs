using System.ComponentModel.DataAnnotations;
using CryptoApi.Objects;
using WebApiPortal.Models.ExchangeRates;

namespace WebApiPortal.Models
{
    public class PricedPair
    {
        [Required(ErrorMessage = "No symbol specified!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "No quote specified!")]
        public string Quote { get; set; }

        public string Exchange { get; set; }
        public double Price { get; set; }
        public PricedPair(){}
        public PricedPair(string name, string quote, string exchange, double price)
        {
            Name = name;
            Quote = quote;
            Exchange = exchange;
            Price = price;
        }

        public PricedPair(CurrentPriceModel pair, double price)
        {
            Name = pair.Name;
            Quote = pair.Quote;
            Exchange = pair.Exchange;
            this.Price = price;
        }

    }
}
