using System.Collections.Generic;
using Teleport.Models;
using Teleport.Repository;

namespace Teleport.Services
{
    public class CommodityService : ICommodityService
    {
        private readonly ICommodityRepo _commodityRepo;

        public CommodityService(ICommodityRepo commodityRepo)
        {
            _commodityRepo = commodityRepo;
        }

        public IEnumerable<Commodity> GetAllCommodities()
        {
            return _commodityRepo.GetAll();
        }

        public void AddCommodity(Commodity commodity)
        {
            _commodityRepo.Add(commodity);
        }

        public void RemoveCommodity(Commodity commodity)
        {
            _commodityRepo.Remove(commodity);
        }
    }
}