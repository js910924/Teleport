using System.Threading.Tasks;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface IShoppingCartRepo
    {
        ShoppingCart GetByCustomerId(int customerId);
        Task Upsert(ShoppingCart shoppingCart);
    }
}