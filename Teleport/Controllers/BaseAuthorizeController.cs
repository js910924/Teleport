using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teleport.Extension;

namespace Teleport.Controllers
{
    [Authorize]
    public class BaseAuthorizeController : Controller
    {
        protected int CustomerId => User.GetCustomerId();
    }
}