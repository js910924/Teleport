using System.Linq;
using System.Security.Claims;

namespace Teleport.Extention
{
    public static class UserExtension
    {
        public static int GetCustomerId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.First(claim => claim.Type == "CustomerId");
            return int.Parse(claim.Value);
        }
    }
}