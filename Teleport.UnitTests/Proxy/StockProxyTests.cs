using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Teleport.Models;
using Teleport.Proxy;

namespace Teleport.UnitTests.Proxy
{
    [TestFixture]
    public class StockProxyTests
    {
        [Test]
        public async Task should_get_stock_info_when_call_GetStockInfo()
        {
            var stockProxy = new StockProxy();

            var stockInfo = stockProxy.GetStockInfo("F");

            //stockInfo.Should().BeEquivalentTo(new StockInfo
            //{
            //    Symbol = "F",
            //    Price = 13.35m,
            //    PercentageOfChange = -0.0015m,
            //    Change = -0.02m
            //});
        }
    }
}