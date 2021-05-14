﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                    CustomerId = customerId,
                    Commodities = Enumerable.Empty<Commodity>().ToList()
                };
            }
   
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ShoppingCart>(json);
        }

        public async Task Upsert(ShoppingCart shoppingCart)
        {
            var json = JsonConvert.SerializeObject(shoppingCart, Formatting.Indented);

            await File.WriteAllTextAsync($"{DirectoryPath}{shoppingCart.CustomerId}_shoppingCart.json", json);
        }
    }
}