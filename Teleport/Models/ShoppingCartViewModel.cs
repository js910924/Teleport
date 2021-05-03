using System.Collections.Generic;

namespace Teleport.Models
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<Commodity> Commodities { get; set; }
    }
}