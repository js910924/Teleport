using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Controllers
{
    public class AccountController : Controller
    {
        private const string DirectoryPath = "./Database/Customer/";

        [HttpGet]
        public ViewResult SignUp()
        {
            return View("SignUp", new AccountViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ViewResult> SignUp(AccountViewModel model)
        {
            if (System.IO.File.Exists($@"{DirectoryPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account already exist";
                return View("SignUp", model);
            }

            var accountInfo = model.ToAccountInfo();
            accountInfo.CustomerId = GetTotalCustomerAmount() + 1;
            await System.IO.File.WriteAllTextAsync($@"{DirectoryPath}{model.Account}.json", JsonConvert.SerializeObject(accountInfo));

            return View("SignIn", model);
        }

        [HttpGet]
        public ViewResult SignIn()
        {
            return View("SignIn", new AccountViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SignIn(AccountViewModel model)
        {
            if (!System.IO.File.Exists($@"{DirectoryPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account not exist";
                return View("SignIn", model);
            }

            var json = await System.IO.File.ReadAllTextAsync($@"{DirectoryPath}{model.Account}.json");
            var accountInfo = JsonConvert.DeserializeObject<AccountInfo>(json);
            if (accountInfo.Account != model.Account || accountInfo.Password != model.Password)
            {
                model.ErrorMessage = "account or password is invalid";
                return View("SignIn", model);
            }

            var claims = new[] { new Claim("CustomerId", accountInfo.CustomerId.ToString()) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(principal,
                new AuthenticationProperties()
                {
                    IsPersistent = false, // logout when close browser
                    ExpiresUtc = DateTime.Now.AddMinutes(60)    // default is 14 days
                });

            return RedirectToAction("History", "Portfolio");
        }

        [Authorize]
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("SignIn");
        }

        private static int GetTotalCustomerAmount()
        {
            var dir = new System.IO.DirectoryInfo(DirectoryPath);
            var count = dir.GetFiles().Length;
            return count;
        }
    }
}