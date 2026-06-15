using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entites;
using ToDoList.Domain;
using System.Data;
using AntiShop.Models;


namespace AntiShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ToDoListContext _context;

        public OrderController(ToDoListContext context)
        {
            _context = context;
        }
        [HttpGet("orders")]
        public IActionResult Orders()
        {
            return View();
        }
        [HttpGet("orders/search")]
        public async Task<IActionResult> Orders(string searchTerm)
        {
            var orders = await _context.Order.ToListAsync();
            var orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderItems = await _context.OrderItems
                    .Where(oi => oi.OrderID == order.Id)
                    .ToListAsync();

                var items = await _context.Items
                    .Where(i => orderItems.Select(oi => oi.ItemID).Contains(i.Id))
                    .ToListAsync();


                orderViewModels.Add(new OrderViewModel
                {
                    Order = order,
                    Items = ConvertToItemViewModels(items) // Преобразование
                });
            }

            return View(orderViewModels); 
        }
        private List<ItemViewModel> ConvertToItemViewModels(List<Item> items)
        {
            return items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
            }).ToList();
        }

        // GET: Order/AddOrder
        public async Task<IActionResult> AddOrder()
        {
            var items = await _context.Items
                .Where(i => i.Inventory > 0)
                .ToListAsync();
            return View(items);
        }

        // POST: Order/AddOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrder(List<int> selectedItemIds, string nameOrder)
        {
            if (selectedItemIds == null || selectedItemIds.Count == 0)
            {
                ModelState.AddModelError("", "Необходимо выбрать хотя бы один товар.");
                return View(await _context.Items.Where(i => i.Inventory > 0).ToListAsync());
            }

            var items = await _context.Items
                .Where(i => selectedItemIds.Contains(i.Id))
                .ToListAsync();

            if (items.Count == 0)
            {
                ModelState.AddModelError("", "Не удалось найти выбранные товары.");
                return View(await _context.Items.Where(i => i.Inventory > 0).ToListAsync());
            }

            var totalAmount = items.Sum(i => i.Price);

            var order = new Order
            {
                OrderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Форматирование даты
                TotalAmount = totalAmount,
                NameOrder = nameOrder
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem
                {
                    OrderID = order.Id,
                    ItemID = item.Id
                };
                _context.OrderItems.Add(orderItem);
                item.Inventory -= 1; // Уменьшаем количество на 1
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Orders"); // Перенаправление на список заказов или другую страницу
        }
      
    }
}
