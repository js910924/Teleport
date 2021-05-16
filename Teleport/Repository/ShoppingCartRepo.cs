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
        private const string DirPath = "/Database/ShoppingCart/";

        public ShoppingCartRepo(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ShoppingCart GetByCustomerId(int customerId)
        {
            EnsureDirectoryExist();

            var filePath = $"{GetDirPath()}{customerId}_shoppingCart.json";
            if (!File.Exists(filePath))
            {
                return new ShoppingCart
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
            EnsureDirectoryExist();

            var json = JsonConvert.SerializeObject(shoppingCart, Formatting.Indented);

            await File.WriteAllTextAsync($"{GetDirPath()}{shoppingCart.CustomerId}_shoppingCart.json", json);
        }

        private void EnsureDirectoryExist()
        {
            if (!Directory.Exists(GetDirPath()))
            {
                Directory.CreateDirectory(GetDirPath());
            }
        }

        private string GetDirPath()
        {
            return $"{_webHostEnvironment.ContentRootPath}{DirPath}";
        }
    }
}