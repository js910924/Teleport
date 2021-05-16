using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
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
}