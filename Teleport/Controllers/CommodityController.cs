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
                }).OrderBy(commodity => commodity.Id).ToList()
            };

            return View("Index", viewModel);
        }

        public IActionResult Add(CommodityOperationRequest request)
        {
            var commodity = new Commodity
            {
                Id = request.Id,
                Title = request.Title,
                Price = request.Price
            };
            _commodityService.AddCommodity(commodity);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(CommodityOperationRequest request)
        {
            var commodity = new Commodity
            {
                Id = request.Id
            };

            _commodityService.RemoveCommodity(commodity);

            return RedirectToAction("Index");
        }
    }
}