using System.Collections.Generic;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<Commodity> Commodities { get; set; }
        public int Id { get; set; }

        public ShoppingCartViewModel ToShoppingCartViewModel()
        {
            return new()
            {
                Commodities = Commodities
            };
        }

        public void AddCommodity(int commodityId)
        {
            Commodities.Add(new Commodity { Id = commodityId });
        }
    }
}