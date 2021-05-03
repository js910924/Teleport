using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Teleport.Models;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.UnitTests.Service
{
    [TestFixture]
    public class ShoppingCartServiceTests
    {
        private ShoppingCartService _shoppingCartService;
        private IShoppingCartRepo _shoppingCartRepo;

        [SetUp]
        public void SetUp()
        {
            _shoppingCartRepo = Substitute.For<IShoppingCartRepo>();

            _shoppingCartService = new ShoppingCartService(_shoppingCartRepo);
        }

        [Test]
        public void should_get_all_commodity_when_call_Index()
        {
            const int customerId = 9487;

            _shoppingCartRepo.GetByCustomerId(customerId).Returns(new ShoppingCart()
            {
                CustomerId = customerId,
                Commodities = new List<Commodity>()
            });

            var shoppingCart = _shoppingCartService.GetCart(customerId);

            shoppingCart.Should().BeEquivalentTo(new ShoppingCart
            {
                CustomerId = customerId,
                Commodities = new List<Commodity>()
            });
        }
    }
}