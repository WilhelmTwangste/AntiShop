using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain.Entites
{
    public class OrderItem : Entity
    {
        public int OrderID { get; set; }
        public int ItemID { get; set; }

        public Order Order { get; set; }
        public Item Item { get; set; }
        public OrderItem() { }
        public OrderItem(int orderId, int itemId)
        {
            OrderID = orderId;
            ItemID = itemId;
        }
    }
}
