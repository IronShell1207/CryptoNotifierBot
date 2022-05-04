using CryptoApi.Objects;
using CryptoBotWebPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CryptoBotWebPortal.Pages
{
    public class CryptoDataModel : PageModel
    {
        public List<PricedTradingPair> pairs { get; set; }
        public  async void OnGet()
        {
            pairs = await new WeatherForecastService().GetLatest100Pairs();
        }
    }
}
