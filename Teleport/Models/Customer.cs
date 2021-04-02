using System;

namespace Teleport.Models
{
    public class Customer
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}