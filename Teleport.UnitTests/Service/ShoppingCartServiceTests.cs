using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const int CustomerId = 9487;
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
            GivenShoppingCart(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity>()
            });

            var shoppingCart = _shoppingCartService.GetCart(CustomerId);

            shoppingCart.Should().BeEquivalentTo(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity>()
            });
        }

        [Test]
        public async Task should_add_commodity_to_customer_shopping_cart_when_call_Add()
        {
            GivenShoppingCart(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity>()
            });

            var shoppingCart = await _shoppingCartService.AddCommodity(CustomerId, 91);

            shoppingCart.Should().BeEquivalentTo(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity> { new() { Id = 91 }},
            });
        }

        private void GivenShoppingCart(ShoppingCart shoppingCart)
        {
            _shoppingCartRepo.GetByCustomerId(CustomerId).Returns(shoppingCart);
        }
    }
}