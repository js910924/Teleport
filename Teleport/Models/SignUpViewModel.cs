using Teleport.Controllers;

namespace Teleport.Models
{
    public class SignUpViewModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public SignUpInfo ToSignUpInfo()
        {
            return new()
            {
                Account = Account,
                Password = Password
            };
        }
    }
}