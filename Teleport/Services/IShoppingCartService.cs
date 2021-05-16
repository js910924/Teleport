using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Services
{
    public interface IShoppingCartService
    {
        ShoppingCart GetCart(int customerId);
        Task<ShoppingCart> AddCommodity(int customerId, ShoppingCartCommodity commodity);
        Task<ShoppingCart> RemoveCommodity(int customerId, ShoppingCartCommodity shoppingCartCommodity);
        Task<ShoppingCart> UpdateCommodity(int customerId, ShoppingCartCommodity shoppingCartCommodity);
    }
}