using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain.Entites
{
    public class Order : Entity
    {
        public string OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string NameOrder { get; set; }
        public Order() { }
        public Order(string orderDate, decimal totalAmount, string nameOrder)
        {
            OrderDate = orderDate;
            TotalAmount = totalAmount;
            NameOrder = nameOrder;
        }
    }

}
