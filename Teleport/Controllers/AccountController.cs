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
        private const string DirectoryPath = "/app/Database/Customer/";

        [HttpGet]
        public ViewResult SignUp()
        {
            return View("SignUp", new SignUpViewModel());
        }

        [HttpPost]
        public async Task<ViewResult> SignUp(SignUpViewModel model)
        {
            if (System.IO.File.Exists($@"{DirectoryPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account already exist";
                return View("SignUp", model);
            }

            var signUpInfo = model.ToSignUpInfo();
            signUpInfo.CustomerId = GetTotalCustomerAmount() + 1;
            await System.IO.File.WriteAllTextAsync($@"{DirectoryPath}{model.Account}.json", JsonConvert.SerializeObject(signUpInfo));

            return View("SignIn", model);
        }

        [HttpGet]
        public ViewResult SignIn()
        {
            return View("SignIn", new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignUpViewModel model)
        {
            if (!System.IO.File.Exists($@"{DirectoryPath}{model.Account}.json"))
            {
                model.ErrorMessage = "account not exist";
                return RedirectToAction("SignUp", model);
            }

            var json = await System.IO.File.ReadAllTextAsync($@"{DirectoryPath}{model.Account}.json");
            var signUpInfo = JsonConvert.DeserializeObject<SignUpInfo>(json);
            if (signUpInfo.Account != model.Account || signUpInfo.Password != model.Password)
            {
                model.ErrorMessage = "account or password is invalid";
                return RedirectToAction("SignUp", model);
            }

            var claims = new[] { new Claim("Account", model.Account) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(principal,
                new AuthenticationProperties()
                {
                    //IsPersistent = false, // logout when close browser
                    ExpiresUtc = DateTime.Now.AddMinutes(60)    // default is 14 days
                });

            return RedirectToAction("History", "Portfolio", new { signUpInfo.CustomerId });
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