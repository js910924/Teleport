namespace Teleport.Models
{
    public class AccountViewModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public Customer ToCustomer()
        {
            return new()
            {
                Account = Account,
                Password = Password
            };
        }
    }
}