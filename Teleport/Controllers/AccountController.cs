using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Teleport.Models;

namespace Teleport.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DirPath = "/Database/Customer/";

        public AccountController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public ViewResult SignUp()
        {
            return View("SignUp", new AccountViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ViewResult> SignUp(AccountViewModel model)
        {
            if (System.IO.File.Exists($"{_webHostEnvironment.ContentRootPath}{DirPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account already exist";
                return View("SignUp", model);
            }

            var customer = model.ToCustomer();
            customer.CustomerId = GetTotalCustomerAmount() + 1;
            customer.CreatedOn = DateTime.Now;
            await System.IO.File.WriteAllTextAsync($"{_webHostEnvironment.ContentRootPath}{DirPath}{model.Account}.json", JsonConvert.SerializeObject(customer));

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
            if (!System.IO.File.Exists($"{_webHostEnvironment.ContentRootPath}{DirPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account not exist";
                return View("SignIn", model);
            }

            var json = await System.IO.File.ReadAllTextAsync($"{_webHostEnvironment.ContentRootPath}{DirPath}{model.Account}.json");
            var customer = JsonConvert.DeserializeObject<Customer>(json);
            if (customer.Account != model.Account || customer.Password != model.Password)
            {
                model.ErrorMessage = "account or password is invalid";
                return View("SignIn", model);
            }

            var claims = new[] { new Claim("CustomerId", customer.CustomerId.ToString()) };
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

        private int GetTotalCustomerAmount()
        {
            var dir = new System.IO.DirectoryInfo($"{_webHostEnvironment.ContentRootPath}{DirPath}");

            return dir.GetFiles().Length;
        }
    }
}