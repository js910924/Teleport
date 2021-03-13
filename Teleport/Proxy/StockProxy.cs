using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Teleport.Models;

namespace Teleport.Proxy
{
    public class StockProxy : IStockProxy
    {
        private readonly HttpClient _httpClient;

        public StockProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://finance.yahoo.com/quote/");
        }

        public StockInfo GetStockInfo(string stockSymbol)
        {
            var response = _httpClient.GetStringAsync(stockSymbol).GetAwaiter().GetResult();
            var regex = new Regex(@"<span class=""Trsdu\(0\.3s\) Fw\(b\) Fz\(36px\) Mb\(-4px\) D\(ib\)"" data-reactid=""32"">([0-9]*.[0-9]*)<\/span>");
            var match = regex.Match(response);
            decimal.TryParse(match.Groups[1].Value, out var price);

            var regex1 = new Regex(@"<span class=""Trsdu\(0\.3s\) Fw\(500\) Pstart\(10px\) Fz\(24px\) C\(.*\)"" data-reactid=""33"">(.*) \((.*)%\)<\/span>");
            var match1 = regex1.Match(response);
            decimal.TryParse(match1.Groups[1].Value, out var percentageOfChange);
            decimal.TryParse(match1.Groups[2].Value, out var change);

            var stockInfo = new StockInfo
            {
                Symbol = stockSymbol,
                Price = price,
                PercentageOfChange = percentageOfChange / 100m,
                Change = change
            };
            return stockInfo;
        }
    }
}