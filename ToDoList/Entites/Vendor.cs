using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain.Entites
{
    public class Vendor : Entity
    {
        public string VendorDate { get; set; }
        public decimal VendorPrice { get; set; }
        public decimal Commission { get; set; }
        public string NameClient { get; set; }
        public Vendor() { }
        public Vendor(string vendorDate, decimal vendorPrice, decimal commission, string nameClient)
        {
            VendorDate = vendorDate;
            VendorPrice = vendorPrice;
            Commission = commission;
            NameClient = nameClient;
        }
    }
}
