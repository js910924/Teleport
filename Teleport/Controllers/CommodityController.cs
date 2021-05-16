using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class CommodityController : Controller
    {
        private readonly ICommodityService _commodityService;

        public CommodityController(ICommodityService commodityService)
        {
            _commodityService = commodityService;
        }

        public IActionResult Index()
        {
            var commodities = _commodityService.GetAllCommodities();

            var viewModel = new CommodityViewModel
            {
                Rows = commodities.Select(commodity => new CommodityRow
                {
                    Id = commodity.Id,
                    Title = commodity.Title,
                    Price = commodity.Price
                }).ToList()
            };

            return View("Index", viewModel);
        }
    }

    public class CommodityViewModel
    {
        public List<CommodityRow> Rows { get; set; }
    }

    public class CommodityRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}