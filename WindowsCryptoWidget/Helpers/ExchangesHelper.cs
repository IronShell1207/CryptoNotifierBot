using CryptoApi.Static.DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Constants;

namespace WindowsCryptoWidget.Helpers
{
    public class ExchangesHelper
    {
        public static DataRequester LocalDataRequester { get; private set; }

        public void StartLoop()
        {
            LocalDataRequester = new DataRequester();
            Task.Run(() => LocalDataRequester.UpdateKucoinLoop());
        }
    }
}