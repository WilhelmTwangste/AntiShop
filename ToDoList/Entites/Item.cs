using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain.Entites
{
    public class Item : Entity
    {
        public int VendorID { get; set; }
        public int DiscountID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string DateDelivery { get; set; }
        public int Inventory { get; set; }

        public Vendor Vendor { get; set; }
        public Discount Discount { get; set; }

        public Item() { }
        public Item(int vendorID, int discountID, string title, string description, decimal price, string date, int inventory)
        {
            VendorID = vendorID; 
            DiscountID = discountID;
            Title = title;
            Description = description;
            Price = price;
            DateDelivery = date;
            Inventory = inventory;
        }
    }
}
