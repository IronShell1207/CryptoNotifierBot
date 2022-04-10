using System;
using System.Threading.Tasks;
using CryptoApi.Static;
using CryptoApi.Objects;

namespace CryptoApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = OkxApi.GetExchangeData();
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
