using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entites;

namespace ToDoList.Domain
{
    public class ToDoListContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskApp> Tasks { get; set; }

        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options)
        {
        }

    }
}
