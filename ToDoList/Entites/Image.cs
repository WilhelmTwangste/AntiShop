using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Domain.Entites
{
    public class Image : Entity
    {
        public int ItemId { get; set; }
        public byte[] ImageData { get; set; }
        public Item Item { get; set; }
        public Image() { } // Конструктор по умолчанию
        public Image(int itemId, byte[] imageData)
        {
            ItemId = itemId;
            ImageData = imageData;
        }
    }
}
