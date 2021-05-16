using System.Threading.Tasks;
using Teleport.Controllers;
using Teleport.Models;
using Teleport.Repository;

namespace Teleport.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepo _shoppingCartRepo;

        public ShoppingCartService(IShoppingCartRepo shoppingCartRepo)
        {
            _shoppingCartRepo = shoppingCartRepo;
        }

        public ShoppingCart GetCart(int customerId)
        {
            return _shoppingCartRepo.GetByCustomerId(customerId);
        }

        public async Task<ShoppingCart> AddCommodity(int customerId, Commodity commodity)
        {
            var shoppingCart = GetCart(customerId);

            shoppingCart.AddCommodity(commodity);

            await _shoppingCartRepo.Upsert(shoppingCart);

            return shoppingCart;
        }

        public async Task<ShoppingCart> RemoveCommodity(int customerId, ShoppingCartCommodity shoppingCartCommodity)
        {
            var shoppingCart = GetCart(customerId);

            shoppingCart.RemoveCommodity(shoppingCartCommodity);

            await _shoppingCartRepo.Upsert(shoppingCart);

            return shoppingCart;
        }
    }
}