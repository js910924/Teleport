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
                Rows = shoppingCart.Commodities.Select(commodity => new ShoppingCartCommodityRow()
                {
                    CommodityId = commodity.Id,
                    CommodityTitle = commodity.Title
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddCommodity(ShoppingCartCommodityOperationRequest shoppingCartCommodityOperationRequest)
        {
            await _shoppingCartService.AddCommodity(CustomerId, new Commodity
            {
                Id = shoppingCartCommodityOperationRequest.CommodityId,
                Title = shoppingCartCommodityOperationRequest.CommodityTitle
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
    }
}