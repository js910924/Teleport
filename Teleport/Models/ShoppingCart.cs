using System.Collections.Generic;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public IEnumerable<Commodity> Commodities { get; set; }
        public int Id { get; set; }

        public ShoppingCartViewModel ToShoppingCartViewModel()
        {
            return new()
            {
                Commodities = Commodities
            };
        }
    }
}