namespace Teleport.Models
{
    public class AccountViewModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public AccountInfo ToAccountInfo()
        {
            return new()
            {
                Account = Account,
                Password = Password
            };
        }
    }
}