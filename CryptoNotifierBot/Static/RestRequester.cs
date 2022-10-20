﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoApi.Constants;
using CryptoApi.Objects;
using RestSharp;

namespace CryptoApi.Static
{
    public class RestRequester : TheDisposable
    {
        public async Task<RestResponse> GetRequest(Uri Link, string exchange)
        {
            try
            {
                var request = new RestRequest(Link);
                request.Method = Method.Get;
                request.Timeout = 10000;
                request.AddHeader("UserAgent", WebHeaders.UserAgent);
                var client = new RestClient();
                var result = await client.ExecuteAsync(request);
                if (result.StatusCode == 0)
                {
                    Diff.LogWrite($"No connection while requesting {exchange} ticker data. Status code: {result.StatusCode}. {result.Content}", ConsoleColor.DarkRed);
                    client = new RestClient(ProxyClient(Link));
                }

                result = await client.ExecuteAsync(request);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private HttpClient ProxyClient(Uri link)
        {
            List<Uri> ProxyList = new List<Uri>()
            {
                new Uri("http://130.41.47.235:8080"),
                new Uri("http://130.41.15.76:8080"),
            };
            foreach (var uri in ProxyList)
            {
                var webProxy = new WebProxy(
                    uri,
                    BypassOnLocal: false);

                var proxyHttpClientHandler = new HttpClientHandler
                {
                    Proxy = webProxy,
                    UseProxy = true,
                };

                var httpclient = new HttpClient(proxyHttpClientHandler)
                {
                    BaseAddress = link,
                    DefaultRequestHeaders = { { "User-agent", WebHeaders.UserAgent } },
                    Timeout = TimeSpan.FromSeconds(10)
                };
                if (CheckProxy(httpclient, uri).Result)
                {
                    return httpclient;
                }
            }

            return new HttpClient()
            {
                BaseAddress = link
            };
        }

        private async Task<bool> CheckProxy(HttpClient client, Uri proxy)
        {
            bool isfirstry = true;
        start:
            try
            {
                var msg = new HttpRequestMessage(HttpMethod.Get, "https://google.com");
                var result = await client.SendAsync(msg);
                if (result.IsSuccessStatusCode)
                {
                    Diff.LogWrite($"Proxy connected: {proxy} {result}", ConsoleColor.Green);
                    return true;
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                if (ex.HResult == -2146232800 && isfirstry == true)
                {
                    Diff.LogWrite($"Proxy error: {proxy} Code: {ex.StatusCode}", ConsoleColor.Red);
                    isfirstry = false;
                    goto start;
                }
                return false;
            }
        }
    }
}