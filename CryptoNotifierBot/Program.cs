using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CryptoApi.Static;
using CryptoApi.Objects;
using CryptoApi.Static.DataHandler;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Json;

namespace CryptoApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Logger(lc => lc
                    .WriteTo.File("first.json")).CreateLogger();

            DataRequester re = new DataRequester();
            re.UpdateParallelly();
            //re.UpdateAllData();

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
