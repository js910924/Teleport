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
    }
}