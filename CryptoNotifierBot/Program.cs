using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Static;
using CryptoApi.Objects;

namespace CryptoApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            // var data = OkxApi.GetExchangeData();
            //Task.Run(()=>
            //{
            //    while (true) ExchangeDataKeeper.DataUpdater();
            //});
            //while (true)
            //{
            //    Console.ReadLine();
            //}
        }
    }
}
