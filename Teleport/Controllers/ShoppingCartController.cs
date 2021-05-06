using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            return View("Index", shoppingCart.ToShoppingCartViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> AddCommodity(int commodityId)
        {
            await _shoppingCartService.AddCommodity(CustomerId, commodityId);

            return RedirectToAction("Index");
        }
    }
}