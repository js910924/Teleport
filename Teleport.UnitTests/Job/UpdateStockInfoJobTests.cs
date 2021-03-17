using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Teleport.Job;
using Teleport.Models;
using Teleport.Proxy;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.UnitTests.Job
{
    [Ignore("Single Responsibility Principle, when this job run, just update all stock info")]
    [TestFixture]
    public class UpdateStockInfoJobTests
    {
        private IStockInfoRepo _stockInfoRepo;
        private IStockMarketChecker _stockMarketChecker;
        private IStockProxy _stockProxy;
        private UpdateAllStockInfoJob _updateAllStockInfoJob;

        [SetUp]
        public void SetUp()
        {
            _stockInfoRepo = Substitute.For<IStockInfoRepo>();
            _stockMarketChecker = Substitute.For<IStockMarketChecker>();
            _stockProxy = Substitute.For<IStockProxy>();

            _updateAllStockInfoJob = new UpdateAllStockInfoJob(_stockProxy, _stockInfoRepo);
        }

        [Test]
        public async Task when_is_close_market_time_and_stockInfo_from_repo_modifiedOn_is_in_close_market_time_should_not_call_StockProxy_GetStockInfo()
        {
            GivenStockInfoFromRepo(new StockInfo { Symbol = "AAPL", Price = 300m, PercentageOfChange = 0.20m, Change = 50m, ModifiedOn = new DateTime(2021, 3, 14, 19, 22, 20) });
            GivenStockMarketOpen(false);
            GivenStockInfoIsInOpenMarket(false);
            GivenStockInfoModifiedOnIsTenSecondsAgo();

            await _updateAllStockInfoJob.Execute(null);

            await _stockProxy.DidNotReceive().GetStockInfo("AAPL");
        }

        [Test]
        public async Task when_is_close_market_time_wont_()
        {
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GivenStockInfoFromRepo(new StockInfo { Symbol = "AAPL", Price = 300m, PercentageOfChange = 0.20m, Change = 50m, ModifiedOn = new DateTime(2021, 3, 14, 19, 22, 20) });
            GivenStockMarketOpen(false);
            GivenStockInfoIsInOpenMarket();
            GivenStockInfoModifiedOnIsTenSecondsAgo(false);

            await _updateAllStockInfoJob.Execute(null);

            await _stockProxy.Received().GetStockInfo("AAPL");
        }

        [Test]
        public async Task when_is_close_market_time_should_not_call_StockProxy_GetStockInfo()
        {
            await _updateAllStockInfoJob.Execute(null);

            await _stockProxy.DidNotReceiveWithAnyArgs().GetStockInfo("AAPL");
        }

        [Test]
        public async Task when_is_open_market_time_and_stock_info_from_database_modifiedOn_is_10_sec_ago_should_call_StockProxy_GetStockInfo()
        {
            GivenStockInfoFromRepo(new StockInfo
            {
                Symbol = "AAPL",
                Price = 300m,
                PercentageOfChange = 0.20m,
                Change = 50m,
                ModifiedOn = new DateTime(2021, 3, 14, 19, 22, 20)
            });
            GiveStockInfoFromProxy(new StockInfo() { Symbol = "AAPL", Price = 200m, PercentageOfChange = 0.0526m, Change = 10m });
            GivenStockMarketOpen();
            GivenStockInfoIsInOpenMarket();
            GivenStockInfoModifiedOnIsTenSecondsAgo();

            await _updateAllStockInfoJob.Execute(null);

            await _stockProxy.Received().GetStockInfo("AAPL");
            await _stockInfoRepo.ReceivedWithAnyArgs().UpsertStockInfo(default);
        }

        [Test]
        public async Task when_is_open_market_time_and_stock_info_from_database_modifiedOn_is_in_open_market_time_is_5_sec_ago_should_not_call_StockProxy_GetStockInfo()
        {
            GivenStockInfoFromRepo(new StockInfo
            {
                Symbol = "AAPL",
                Price = 300m,
                PercentageOfChange = 0.20m,
                Change = 50m
            });
            GivenStockMarketOpen();
            GivenStockInfoIsInOpenMarket();
            GivenStockInfoModifiedOnIsTenSecondsAgo(false);

            await _updateAllStockInfoJob.Execute(null);

            await _stockProxy.DidNotReceive().GetStockInfo("AAPL");
            await _stockInfoRepo.DidNotReceiveWithAnyArgs().UpsertStockInfo(default);
        }

        [Test]
        public async Task when_is_open_market_time_and_stock_info_from_database_modifiedOn_is_in_close_market_time_is_5_sec_ago_should_not_call_StockProxy_GetStockInfo()
        {
            GivenStockInfoFromRepo(new StockInfo
            {
                Symbol = "AAPL",
                Price = 300m,
                PercentageOfChange = 0.20m,
                Change = 50m
            });
            GivenStockMarketOpen();
            GivenStockInfoIsInOpenMarket(false);
            GivenStockInfoModifiedOnIsTenSecondsAgo(false);

            await _stockProxy.DidNotReceive().GetStockInfo("AAPL");
            await _stockInfoRepo.DidNotReceiveWithAnyArgs().UpsertStockInfo(default);
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

        private void GiveStockInfoFromProxy(StockInfo stockInfo)
        {
            _stockProxy.GetStockInfo(stockInfo.Symbol)
                .Returns(Task.FromResult(stockInfo));
        }
    }
}