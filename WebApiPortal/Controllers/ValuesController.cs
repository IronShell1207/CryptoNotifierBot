using CryptoApi.API;
using CryptoApi.CoinMarketCapData;
using CryptoApi.Objects;
using CryptoApi.Objects.ExchangesPairs;
using CryptoApi.Static.DataHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPortal.Models;
using WebApiPortal.Models.ExchangeRates;

namespace WebApiPortal.Controllers
{
    [Route("crypto")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<string>> GetCurrentPrice(string name, string quote, string exchange)
        {
            using (CryptoApi.Static.DataHandler.DataRequester requester = new DataRequester())
            {
                var info = await requester.GetCurrentPricePairByName(name, quote, exchange);
                if (info != null)
                {
                    return $"{info.Name}/{info.Quote} - {info.Price}. Dbset: {info.CryDbSetId}";
                }
                else return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostCurrentPrice([FromBody] TradingPair pair)
        {
            using (CryptoApi.Static.DataHandler.DataRequester requester = new DataRequester())
            {
                var info = await requester.GetCurrentPricePairByName(pair);
                if (info != null)
                {
                    return StatusCode(StatusCodes.Status200OK, info);
                }
                else return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            }
        }
        [HttpPost]
        [Route("getKucoinData")]
        public async Task<ActionResult<KuTickerDB>> GetKucoinPair(string name, string quote)
        {
            using (DataRequester requester = new())
            {
                var info = await requester.GetKucoinData(name, quote);
                if (info == null)
                    return StatusCode(StatusCodes.Status404NotFound, 
                        new Response { Status = "Not found!", Message = "Pair not found in kucoin data!" });
                return StatusCode(StatusCodes.Status200OK, info);
            }
        }

    }
}
