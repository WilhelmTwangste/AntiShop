using ToDoList.Domain.Entites;

namespace AntiShop.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<ItemViewModel> Items { get; set; } // Добавьте это свойство
    }

    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}

