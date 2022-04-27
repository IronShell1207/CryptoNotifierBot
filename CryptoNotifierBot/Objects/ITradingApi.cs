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
        public Task GetExchangeData();

        
        //  public  List<CryptoExchangePairInfo> ExchangePairsConverter(List<object> list);
    }
}
