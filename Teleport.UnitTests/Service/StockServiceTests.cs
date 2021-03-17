using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.UnitTests.Service
{
    [TestFixture]
    public class StockServiceTests
    {
        private IStockProxy _stockProxy;
        private StockService _stockService;
        private IStockInfoRepo _stockInfoRepo;
        private IStockMarketChecker _stockMarketChecker;
        private IStockTransactionService _stockTransactionService;

        [SetUp]
        public void SetUp()
        {
            _stockProxy = Substitute.For<IStockProxy>();
            _stockInfoRepo = Substitute.For<IStockInfoRepo>();
            _stockMarketChecker = Substitute.For<IStockMarketChecker>();
            _stockTransactionService = Substitute.For<IStockTransactionService>();

            _stockService = new StockService(_stockProxy, _stockInfoRepo, _stockMarketChecker, _stockTransactionService);
        }

        [Test]
        public async Task should_convert_all_history_stock_transactions_to_stock_position()
        {
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GivenAllStockTransaction(new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 190m });
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetStockPositionsBy(0);

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 1,
                    AveragePrice = 190m,
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
        public async Task should_convert_all_history_stock_transactions_to_stock_position_with_average_price()
        {
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "TSLA", Price = 500m, PercentageOfChange = -0.1667m, Change = -100m });
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 600m, });
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetStockPositionsBy(0);

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 4,
                    AveragePrice = 177.5m,
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
                    AveragePrice = 650m,
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

        [Test]
        public async Task stock_position_will_only_show_transaction_action_which_is_buy_or_sell()
        {
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "TSLA", Price = 500m, PercentageOfChange = -0.1667m, Change = -100m });
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Buy, Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Buy, Quantity = 1, Price = 600m, },
                new StockTransaction { Ticker = "Cash", Action = StockAction.Deposit, Quantity = 10000, Price = 1m, },
                new StockTransaction { Ticker = "Cash", Action = StockAction.Withdraw, Quantity = 500, Price = 1m, }
                );
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetStockPositionsBy(0);

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 4,
                    AveragePrice = 177.5m,
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
                    AveragePrice = 650m,
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

        [Test]
        public async Task should_not_show_in_position_when_stock_shares_is_zero()
        {
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "TSLA", Price = 500m, PercentageOfChange = -0.1667m, Change = -100m });
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Buy, Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Sell, Quantity = 1, Price = 600m, }
                );
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetStockPositionsBy(0);

            stockPositions.Should().BeEquivalentTo(new List<StockPosition>()
            {
                new StockPosition()
                {
                    Ticker = "AAPL",
                    Shares = 4,
                    AveragePrice = 177.5m,
                    CurrentPrice = 200m,
                    PercentageOfChange = 0.0526m,
                    Change = 10m,
                    Cost = 710m,
                    CurrentValue = 800m,
                    PercentageOfGain = 0.1268m,
                    Gain = 22.5m * 4
                }
            });
        }

        private void GivenStockInfoModifiedOnIsTenSecondsAgo(bool isTenSecondsAgo = true)
        {
            _stockMarketChecker.IsTenSecondsAgo(Arg.Any<DateTime>()).Returns(isTenSecondsAgo);
        }

        private void GivenStockInfoIsInOpenMarket(bool isInOpenMarket = true)
        {
            _stockMarketChecker.IsInOpenMarket(Arg.Any<DateTime>()).Returns(isInOpenMarket);
        }

        private void GivenStockMarketOpen(bool isOpen = true)
        {
            _stockMarketChecker.IsOpenMarket().Returns(isOpen);
        }

        private void GivenStockInfoFromRepo(StockInfo stockInfo)
        {
            _stockInfoRepo.GetStockInfo(stockInfo.Symbol).Returns(stockInfo);
        }

        private void GivenStockInfoNotInDatabase()
        {
            _stockInfoRepo.GetStockInfo(Arg.Any<string>()).Returns(new StockInfo() {Symbol = "NOT IN DATABASE"});
        }

        private void GivenAllStockTransaction(params StockTransaction[] transactions)
        {
            _stockTransactionService.GetStockTransactionsBy(Arg.Any<int>()).Returns(Task.FromResult((IEnumerable<StockTransaction>)transactions));
        }

        private void GiveStockInfoFromProxy(StockInfo stockInfo)
        {
            _stockProxy.GetStockInfo(stockInfo.Symbol)
                .Returns(Task.FromResult(stockInfo));
        }
    }
}