using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain.Entites
{
    public class Discount : Entity
    {
        public string Description { get; set; }
        public decimal Percents { get; set; } // Процент от 0 до 100
        public Discount() { }
        public Discount(string description, decimal percents)
        {
            Description = description;
            Percents = percents;
        }
    }
}
