using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Teleport.Controllers;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Repository;

namespace Teleport.UnitTests.Controller
{
    [TestFixture]
    public class PortfolioControllerTests
    {
        private PortfolioController _controller;
        private IStockTransactionRepo _stockTransactionRepo;
        private IStockProxy _stockProxy;

        [SetUp]
        public void SetUp()
        {
            _stockTransactionRepo = Substitute.For<IStockTransactionRepo>();
            _stockProxy = Substitute.For<IStockProxy>();

            _controller = new PortfolioController(_stockTransactionRepo, _stockProxy);
        }

        [Test]
        public async Task should_convert_all_history_stock_transactions_to_stock_position()
        {
            _stockTransactionRepo.GetAllStockTransactions().Returns(new List<StockTransaction>()
            {
                new StockTransaction
                {
                    Ticker = "AAPL",
                    Quantity = 1,
                    Price = 190m,
                }
            });
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);

            var viewResult = await _controller.Position();

            //stockTransactionRepo.GetAllStockTransactions()

            ((IEnumerable<StockPosition>)viewResult.Model).Should().BeEquivalentTo(new List<StockPosition>()
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
        public async Task should_convert_all_history_stock_transactions_to_stock_position_with_average_purchase_price()
        {
            _stockTransactionRepo.GetAllStockTransactions().Returns(new List<StockTransaction>()
            {
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 600m, },
            });
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GiveStockInfo("TSLA", 500m, -0.1667m, -100m);

            var viewResult = await _controller.Position();

            ((IEnumerable<StockPosition>)viewResult.Model).Should().BeEquivalentTo(new List<StockPosition>()
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
              _stockProxy.GetStockInfo(stockSymbol).Returns(new StockInfo() {Symbol = stockSymbol, Price = price, PercentageOfChange = percentageOfChange, Change = change});
        }
    }
}