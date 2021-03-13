using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Services;

namespace Teleport.UnitTests.Service
{
    [TestFixture]
    public class StockServiceTests
    {
        private IStockProxy _stockProxy;
        private StockService _stockService;

        [SetUp]
        public void SetUp()
        {
            _stockProxy = Substitute.For<IStockProxy>();

            _stockService = new StockService(_stockProxy);
        }

        [Test]
        public void should_convert_all_history_stock_transactions_to_stock_position()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);

            var stockPositions = _stockService.GetAllStockPositions(new List<StockTransaction>()
            {
                new StockTransaction
                {
                    Ticker = "AAPL",
                    Quantity = 1,
                    Price = 190m,
                }
            });

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 1,
                    AveragePurchasePrice = 190m,
                    CurrentPrice = 200m,
                    PercentageOfChange = 0.0526m,
                    Change = 10m,
                    Cost = 190m,
                    CurrentValue = 200m,
                    PercentageOfGain = 0.0526m,
                    Gain = 10m
                }
            });
        }

        [Test]
        public void should_convert_all_history_stock_transactions_to_stock_position_with_average_purchase_price()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GiveStockInfo("TSLA", 500m, -0.1667m, -100m);

            var stockPositions = _stockService.GetAllStockPositions(new List<StockTransaction>()
            {
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 600m, },
            });

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 4,
                    AveragePurchasePrice = 177.5m,
                    CurrentPrice = 200m,
                    PercentageOfChange = 0.0526m,
                    Change = 10m,
                    Cost = 710m,
                    CurrentValue = 800m,
                    PercentageOfGain = 0.1268m,
                    Gain = 22.5m * 4
                },
                new StockPosition
                {
                    Ticker = "TSLA",
                    Shares = 2,
                    AveragePurchasePrice = 650m,
                    CurrentPrice = 500m,
                    PercentageOfChange = -0.1667m,
                    Change = -100m,
                    Cost = 1300m,
                    CurrentValue = 1000m,
                    PercentageOfGain = -0.2308m,
                    Gain = -300m
                }
            });
        }

        private void GiveStockInfo(string stockSymbol, decimal price, decimal percentageOfChange, decimal change)
        {
            _stockProxy.GetStockInfo(stockSymbol).Returns(
                Task.FromResult(new StockInfo() { Symbol = stockSymbol, Price = price, PercentageOfChange = percentageOfChange, Change = change }));
        }
    }
}