using System.Collections.Generic;
using System.Linq;
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

        private readonly Commodity _bookCommodity = CreateCommodity(91, "book");
        private ShoppingCart _shoppingCart;

        [SetUp]
        public void SetUp()
        {
            _shoppingCartRepo = Substitute.For<IShoppingCartRepo>();

            _shoppingCart = new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity>(),
                ShoppingCartCommodities = new List<ShoppingCartCommodity>()
            };
            _shoppingCartService = new ShoppingCartService(_shoppingCartRepo);
        }

        [Test]
        public void should_get_all_commodity_when_call_Index()
        {
            GivenShoppingCart();

            var shoppingCart = _shoppingCartService.GetCart(CustomerId);

            shoppingCart.Should().BeEquivalentTo(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity>(),
                ShoppingCartCommodities = new List<ShoppingCartCommodity>()
            });
        }

        [Test]
        public async Task should_add_commodity_to_customer_shopping_cart_when_call_Add()
        {
            GivenShoppingCart();

            var shoppingCart = await _shoppingCartService.AddCommodity(CustomerId, CreateCommodity(91, "book"));

            shoppingCart.Should().BeEquivalentTo(new ShoppingCart
            {
                CustomerId = CustomerId,
                Commodities = new List<Commodity> {CreateCommodity(91, "book")},
                ShoppingCartCommodities = new List<ShoppingCartCommodity>()
            });
        }

        [Test]
        public async Task should_remove_commodity_with_specific_quantities()
        {
            GivenShoppingCartCommodities(CreateShoppingCartCommodity(_bookCommodity, 5));
            GivenShoppingCart();

            var shoppingCart = await _shoppingCartService.RemoveCommodity(CustomerId, CreateShoppingCartCommodity(_bookCommodity, 2));

            shoppingCart.ShoppingCartCommodities
                .Should()
                .BeEquivalentTo(new List<ShoppingCartCommodity>
                {
                    CreateShoppingCartCommodity(_bookCommodity, 3)
                } );
        }

        [Test]
        public async Task should_remove_commodity_in_shopping_cart_when_quantity_is_0()
        {
            GivenShoppingCartCommodities(CreateShoppingCartCommodity(_bookCommodity, 5));
            GivenShoppingCart();

            var shoppingCart = await _shoppingCartService.RemoveCommodity(CustomerId, CreateShoppingCartCommodity(_bookCommodity, 5));

            shoppingCart.ShoppingCartCommodities
                .Should()
                .BeEquivalentTo(new List<ShoppingCartCommodity>());
        }

        private static Commodity CreateCommodity(int id, string title)
        {
            return new()
            {
                Id = id,
                Title = title
            };
        }

        private static ShoppingCartCommodity CreateShoppingCartCommodity(Commodity commodity, int quantity)
        {
            return new()
            {
                Commodity = commodity,
                Quantity = quantity
            };
        }

        private void GivenShoppingCartCommodities(params ShoppingCartCommodity[] shoppingCartCommodities)
        {
            _shoppingCart.ShoppingCartCommodities = shoppingCartCommodities.ToList();
        }

        private void GivenShoppingCart()
        {
            _shoppingCartRepo.GetByCustomerId(CustomerId).Returns(_shoppingCart);
        }
    }
}