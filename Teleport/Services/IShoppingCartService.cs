using Teleport.Controllers;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IShoppingCartService
    {
        ShoppingCart GetCart(int customerId);
    }
}