using System.Threading.Tasks;
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

        private static int GetTotalCustomerAmount()
        {
            var dir = new System.IO.DirectoryInfo(DirectoryPath);
            var count = dir.GetFiles().Length;
            return count;
        }
    }
}