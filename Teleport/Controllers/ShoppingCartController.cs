using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Teleport.Models;
using Teleport.Services;

namespace Teleport.Controllers
{
    public class ShoppingCartController : BaseAuthorizeController
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var shoppingCart = _shoppingCartService.GetCart(CustomerId);

            return View("Index", new ShoppingCartRowsViewModel
            {
                Rows = shoppingCart.ShoppingCartCommodities.Select(commodity => new ShoppingCartCommodityRow()
                {
                    CommodityId = commodity.Commodity.Id,
                    CommodityTitle = commodity.Commodity.Title,
                    Quantity = commodity.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddCommodity(ShoppingCartCommodityOperationRequest shoppingCartCommodityOperationRequest)
        {
            await _shoppingCartService.AddCommodity(CustomerId, new ShoppingCartCommodity
            {
                Commodity = new Commodity
                {
                    Id = shoppingCartCommodityOperationRequest.CommodityId
                },
                Quantity = shoppingCartCommodityOperationRequest.Quantity
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> RemoveCommodity(ShoppingCartCommodityOperationRequest shoppingCartCommodityOperationRequest)
        {
            await _shoppingCartService.RemoveCommodity(CustomerId, new ShoppingCartCommodity
            {
                Commodity = new Commodity
                {
                    Id = shoppingCartCommodityOperationRequest.CommodityId
                },
                Quantity = shoppingCartCommodityOperationRequest.Quantity
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCommodity(ShoppingCartCommodityOperationRequest shoppingCartCommodityOperationRequest)
        {
            await _shoppingCartService.UpdateCommodity(CustomerId, new ShoppingCartCommodity
            {
                Commodity = new Commodity
                {
                    Id = shoppingCartCommodityOperationRequest.CommodityId
                },
                Quantity = shoppingCartCommodityOperationRequest.Quantity
            });

            return RedirectToAction("Index");
        }
    }
}