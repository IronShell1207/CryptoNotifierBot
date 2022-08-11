using System.ComponentModel.DataAnnotations;
using CryptoApi.Objects;

namespace WebApiPortal.Models.ExchangeRates
{
    public class CurrentPriceModel
    {
        [Required(ErrorMessage = "No symbol specified!")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "No quote specified!")]
        public string? Quote { get; set; }
        [Required(ErrorMessage = "No quote specified!")]
        public string? Exchange { get; set; }
    }
}
