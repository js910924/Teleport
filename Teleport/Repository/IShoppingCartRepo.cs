using Teleport.Controllers;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IShoppingCartRepo
    {
        ShoppingCart GetByCustomerId(int customerId);
    }
}