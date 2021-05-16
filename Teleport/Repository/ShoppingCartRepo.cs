using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Repository
{
    public class ShoppingCartRepo : IShoppingCartRepo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirectoryPath = "/Database/ShoppingCart/";

        public ShoppingCartRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ShoppingCart GetByCustomerId(int customerId)
        {
            var filePath = $"{_webHostEnvironment.ContentRootPath}{DirectoryPath}{customerId}_shoppingCart.json";
            if (!File.Exists(filePath))
            {
                return new ShoppingCart()
                {
                    CustomerId = customerId,
                    ShoppingCartCommodities = Enumerable.Empty<ShoppingCartCommodity>().ToList()
                };
            }
   
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ShoppingCart>(json);
        }

        public async Task Upsert(ShoppingCart shoppingCart)
        {
            var json = JsonConvert.SerializeObject(shoppingCart, Formatting.Indented);

            await File.WriteAllTextAsync($"{_webHostEnvironment.ContentRootPath}{DirectoryPath}{shoppingCart.CustomerId}_shoppingCart.json", json);
        }
    }
}