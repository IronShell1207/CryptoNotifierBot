﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoApi.Objects
{
    public interface ITradingApi
    {
        public SymbolTimedExInfo GetExchangeData();
        //  public  List<CryptoExchangePairInfo> ExchangePairsConverter(List<object> list);
    }
}