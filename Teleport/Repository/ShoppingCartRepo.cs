using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class ShoppingCartRepo : IShoppingCartRepo
    {
        private const string DirectoryPath = @"./Database/ShoppingCart/";

        public ShoppingCart GetByCustomerId(int customerId)
        {
            var filePath = $"{DirectoryPath}{customerId}_shoppingCart.json";
            if (!File.Exists(filePath))
            {
                return new ShoppingCart()
                {
                    Id = 0,
                    CustomerId = customerId,
                    Commodities = Enumerable.Empty<Commodity>()
                };
            }
   
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ShoppingCart>(json);
        }
    }
}