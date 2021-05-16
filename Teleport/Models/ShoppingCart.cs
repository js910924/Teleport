using System.Collections.Generic;
using System.Linq;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<ShoppingCartCommodity> ShoppingCartCommodities { get; set; }

        public void AddCommodity(ShoppingCartCommodity shoppingCartCommodity)
        {
            var commodityInCart = ShoppingCartCommodities.FirstOrDefault(commodity => commodity.Commodity.Id == shoppingCartCommodity.Commodity.Id);
            if (commodityInCart == null)
            {
                ShoppingCartCommodities.Add(shoppingCartCommodity);
            }
        }

        public void RemoveCommodity(ShoppingCartCommodity shoppingCartCommodity)
        {
            var commodityInCart = ShoppingCartCommodities.FirstOrDefault(commodity => commodity.Commodity.Id == shoppingCartCommodity.Commodity.Id);
            if (commodityInCart != null)
            {
                ShoppingCartCommodities.Remove(commodityInCart);
            }
        }

        public void UpdateCommodity(ShoppingCartCommodity shoppingCartCommodity)
        {
            var commodityInCart = ShoppingCartCommodities.First(commodity => commodity.Commodity.Id == shoppingCartCommodity.Commodity.Id);

            commodityInCart.Quantity = shoppingCartCommodity.Quantity;
        }
    }
}