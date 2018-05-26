using System;
using System.Collections.Generic;

namespace DevExpress.Data.Extensions.Tests {
    public class TestContext {
        public string Name { get; set; }
        public int Roles { get; set; }
        public int Age { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int ZipCode { get; set; }
        public double Angle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Account> Accounts {get;} = new List<Account>();
        public List<Order> Orders { get; } = new List<Order>();
    }

    public class Account {
        public decimal Amount { get; set; }
    }

    public class Order {
        public DateTime Date { get; set; }
    }
}
