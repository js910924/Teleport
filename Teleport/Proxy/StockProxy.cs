using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Proxy
{
    public class StockProxy : IStockProxy
    {
        private const string YahooFinancePricePattern = @"<span class=""Trsdu\(0\.3s\) Fw\(b\) Fz\(36px\) Mb\(-4px\) D\(ib\)"" data-reactid=""32"">([0-9]*,?[0-9]*.[0-9]*)<\/span>";
        private const string YahooFinanceDailyChangePattern = @"<span class=""Trsdu\(0\.3s\) Fw\(500\) Pstart\(10px\) Fz\(24px\) C\(.*\)"" data-reactid=""33"">(.*) \((.*)%\)<\/span>";
        private readonly HttpClient _httpClient;

        public StockProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://finance.yahoo.com/quote/");
        }

        public async Task<StockInfo> GetStockInfo(string stockSymbol)
        {
            var html = await _httpClient.GetStringAsync(stockSymbol);

            var priceRegex = new Regex(YahooFinancePricePattern);
            var dailyChangeRegex = new Regex(YahooFinanceDailyChangePattern);
            var priceMatch = priceRegex.Match(html);
            var dailyChangeMatch = dailyChangeRegex.Match(html);

            if (priceMatch.Success && dailyChangeMatch.Success)
            {
                var price = Convert.ToDecimal(priceMatch.Groups[1].Value);
                var change = Convert.ToDecimal(dailyChangeMatch.Groups[1].Value);
                var percentageOfChange = Convert.ToDecimal(dailyChangeMatch.Groups[2].Value);

                return new StockInfo
                {
                    Symbol = stockSymbol,
                    Price = price,
                    PercentageOfChange = percentageOfChange / 100m,
                    Change = change
                };
            }

            return new StockInfo()
            {
                Symbol = stockSymbol
            };
        }
    }
}