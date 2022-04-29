using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Static.DataHandler;


namespace CryptoApi.Objects
{
    public interface ITradingApi 
    {
        public Task GetExchangeData(Guid guid);
        public string ApiName { get; }
        public int PairsCount { get; }
        public DateTime LastUpdate { get; }
        
        //  public  List<CryptoExchangePairInfo> ExchangePairsConverter(List<object> list);
    }
}
