using System;
using System.Collections.Generic;
using System.Linq;

namespace Teleport.Models
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<ShoppingCartCommodity> ShoppingCartCommodities { get; set; }

        public void AddCommodity(ShoppingCartCommodity cartCommodity)
        {
            var shoppingCartCommodity = ShoppingCartCommodities.FirstOrDefault(commodity => commodity.Commodity.Id == cartCommodity.Commodity.Id);
            if (shoppingCartCommodity == null)
            {
                ShoppingCartCommodities.Add(cartCommodity);
            }
            else
            {
                shoppingCartCommodity.Quantity += cartCommodity.Quantity;
            }
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