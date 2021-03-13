using NUnit.Framework;
using Teleport.Controllers;
using Teleport.Repository;

namespace Teleport.UnitTests.Controller
{
    [TestFixture]
    public class PortfolioControllerTests
    {
        [Test]
        public void should_get_all_history_transactions_when_call_Position()
        {
            var stockTransactionRepo = NSubstitute.Substitute.For<IStockTransactionRepo>();

            var controller = new PortfolioController(stockTransactionRepo);
        }
        
    }
}