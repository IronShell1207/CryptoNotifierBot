using CryptoApi.Objects;
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
    [Authorize]
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
    }
}
 