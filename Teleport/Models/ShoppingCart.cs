using System.Collections.Generic;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<Commodity> Commodities { get; set; }

        public void AddCommodity(Commodity commodity)
        {
            Commodities.Add(commodity);
        }
    }
}