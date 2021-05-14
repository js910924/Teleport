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
        public async Task<ActionResult> AddCommodity(AddCommodityRequest addCommodityRequest)
        {
            await _shoppingCartService.AddCommodity(CustomerId, new Commodity
            {
                Id = addCommodityRequest.CommodityId,
                Title = addCommodityRequest.CommodityTitle
            });

            return RedirectToAction("Index");
        }
    }
}