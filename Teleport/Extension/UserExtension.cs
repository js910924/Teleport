using System.Linq;
using System.Security.Claims;

namespace Teleport.Extension
{
    public static class UserExtension
    {
        public static int GetCustomerId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.First(customerIdClaim => customerIdClaim.Type == "CustomerId");
            return int.Parse(claim.Value);
        }

        public static bool IsLogIn(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal != null && claimsPrincipal.Identity.IsAuthenticated;
        }
    }
}