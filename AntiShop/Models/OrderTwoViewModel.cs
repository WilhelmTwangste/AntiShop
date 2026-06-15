using ToDoList.Domain.Entites;

namespace AntiShop.Models
{
    public class OrderTwoViewModel
    {
        public string OrderDate { get; set; } // Дата заказа
        public decimal TotalAmount { get; set; } // Общая сумма заказа
        public string NameOrder { get; set; } // Название заказа
    }
}
