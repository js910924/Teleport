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
        private IStockTransactionRepo _stockTransactionRepo;
        private IStockInfoRepo _stockInfoRepo;

        [SetUp]
        public void SetUp()
        {
            _stockProxy = Substitute.For<IStockProxy>();
            _stockTransactionRepo = Substitute.For<IStockTransactionRepo>();
            _stockInfoRepo = Substitute.For<IStockInfoRepo>();

            _stockService = new StockService(_stockProxy, _stockTransactionRepo, _stockInfoRepo);
        }

        [Test]
        public async Task should_convert_all_history_stock_transactions_to_stock_position()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GivenAllStockTransaction(new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 190m });
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetAllStockPositions();

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
        public async Task should_convert_all_history_stock_transactions_to_stock_position_with_average_purchase_price()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GiveStockInfo("TSLA", 500m, -0.1667m, -100m);
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Quantity = 1, Price = 600m, });
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetAllStockPositions();

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

        [Test]
        public async Task stock_position_will_only_show_transaction_action_which_is_buy_or_sell()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GiveStockInfo("TSLA", 500m, -0.1667m, -100m);
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

            var stockPositions = await _stockService.GetAllStockPositions();

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

        [Test]
        public async Task should_not_show_in_position_when_stock_shares_is_zero()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GiveStockInfo("TSLA", 500m, -0.1667m, -100m);
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 2, Price = 180m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Buy, Quantity = 1, Price = 700m, },
                new StockTransaction { Ticker = "TSLA", Action = StockAction.Sell, Quantity = 1, Price = 600m, }
                );
            GivenStockInfoNotInDatabase();

            var stockPositions = await _stockService.GetAllStockPositions();

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
                }
            });
        }

        [Test]
        public async Task should_not_call_stockProxy_GetStockInfo_when_has_real_time_data_in_database()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 2, Price = 180m, }
                );
            _stockInfoRepo.GetStockInfo("AAPL")
                .Returns(new StockInfo { Symbol = "AAPL", Price = 300m, PercentageOfChange = 0.20m, Change = 50m } );

            await _stockService.GetAllStockPositions();

            await _stockProxy.DidNotReceive().GetStockInfo("AAPL");
        }

        [Test]
        public async Task when_no_stock_info_in_database_should_insert_stock_info_to_database_after_GetStockInfo_from_StockProxy()
        {
            GiveStockInfo("AAPL", 200m, 0.0526m, 10m);
            GivenAllStockTransaction(
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 200m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 1, Price = 150m, },
                new StockTransaction { Ticker = "AAPL", Action = StockAction.Buy, Quantity = 2, Price = 180m, }
                );
            GivenStockInfoNotInDatabase();
            StockInfo stockInfo = null;
            await _stockInfoRepo.UpsertStockInfo(Arg.Do<StockInfo>(info => stockInfo = info));

            await _stockService.GetAllStockPositions();

            Received.InOrder(() =>
            {
                _stockProxy.GetStockInfo("AAPL");
                _stockInfoRepo.UpsertStockInfo(Arg.Any<StockInfo>());
            });

            stockInfo.Should().BeEquivalentTo(new StockInfo
            {
                Symbol = "AAPL",
                Price = 200m,
                PercentageOfChange = 0.0526m,
                Change = 10m
            });
        }

        private void GivenStockInfoNotInDatabase()
        {
            _stockInfoRepo.GetStockInfo(Arg.Any<string>()).Returns(new StockInfo() {Symbol = "NOT IN DATABASE"});
        }

        private void GivenAllStockTransaction(params StockTransaction[] transactions)
        {
            _stockTransactionRepo.GetAllStockTransactions().Returns(Task.FromResult((IEnumerable<StockTransaction>)transactions));
        }

        private void GiveStockInfo(string stockSymbol, decimal price, decimal percentageOfChange, decimal change)
        {
            _stockProxy.GetStockInfo(stockSymbol).Returns(
                Task.FromResult(new StockInfo() { Symbol = stockSymbol, Price = price, PercentageOfChange = percentageOfChange, Change = change }));
        }
    }
}