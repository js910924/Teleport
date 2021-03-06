using System.Collections.Generic;
using Teleport.Models;

namespace Teleport.Repository
{
    public interface ICommodityRepo
    {
        IEnumerable<Commodity> GetAll();
        void Upsert(Commodity commodity);
        void Remove(Commodity commodity);
    }
}