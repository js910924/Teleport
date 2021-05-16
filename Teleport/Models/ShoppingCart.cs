using System;
using System.Collections.Generic;
using System.Linq;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<Commodity> Commodities { get; set; }
        public List<ShoppingCartCommodity> ShoppingCartCommodities { get; set; }

        public void AddCommodity(Commodity commodity)
        {
            Commodities.Add(commodity);
        }

        public void RemoveCommodity(ShoppingCartCommodity shoppingCartCommodity)
        {
            var cartCommodity = ShoppingCartCommodities.First(commodity => commodity.Commodity.Id == shoppingCartCommodity.Commodity.Id);

            var quantity = Math.Max(cartCommodity.Quantity - shoppingCartCommodity.Quantity, 0);
            if (quantity == 0)
            {
                ShoppingCartCommodities.Remove(cartCommodity);
            }
            else
            {
                cartCommodity.Quantity = quantity;
            }
        }
    }
}