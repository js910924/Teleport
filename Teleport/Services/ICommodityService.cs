using System.Collections.Generic;
using Teleport.Models;

namespace Teleport.Services
{
    public interface ICommodityService
    {
        IEnumerable<Commodity> GetAllCommodities();
        void AddCommodity(Commodity commodity);
        void RemoveCommodity(Commodity commodity);
    }
}