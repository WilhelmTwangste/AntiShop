using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entites;
using ToDoList.Domain;
using System.Data;
using AntiShop.Models; 
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace AntiShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ToDoListContext _context;

        public AdminController(ToDoListContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userCount = _context.Users.Count();
            var adminCount = _context.Users.Count(u => u.Role == "Admin");
            var itemCount = _context.Users.Count(u => u.Role == "WarehouseKeeper");
            var accountantCount = _context.Users.Count(u => u.Role == "Accountant");

            var model = new AdminStatisticsViewModel
            {
                UserCount = userCount,
                AdminCount = adminCount,
                ItemCount = itemCount,
                AccountantCount = accountantCount
            };

            return View(model);
            // return View(); // Возвращает представление для администратора
        }

        public IActionResult AddUser()
        {
            var model = new UserViewModel
            {
                User = _context.Users.ToList() // Получаем список пользователей
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(string login, string password, string role)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "Все поля должны быть заполнены.");
                return View("AddUser");
            }

            // Создание нового пользователя
            var user = new User(login, password, role);

            // Добавление пользователя в базу данных
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Уведомление об успешном добавлении
            ViewBag.SuccessMessage = "Учетная запись успешно добавлена!";

            // Очистка полей формы
            ModelState.Clear();
            return View("AddUser"); // Возвращает представление для добавления пользователя
        }
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                ViewBag.SuccessMessage = "Пользователь успешно удален.";
            }
            return RedirectToAction("AddUser");
        }
        public IActionResult Items(string searchTerm)
        {
            var items = _context.Items.Include(i => i.Vendor).Include(i => i.Discount).ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(p => p.Title.Contains(searchTerm)).ToList(); // Фильтрация по названию
            }

            if (!items.Any())
            {
                ViewBag.Message = "Товара с таким названием нет в наличии.";
            }

            return View(items); // Возврат представления с отфильтрованными товарами
        }
        // Метод для добавления нового товара
        public IActionResult AddItem()
        {
            ViewBag.Vendor = _context.Vendor.ToList(); // Получаем список поставщиков
            ViewBag.Discount = _context.Discount.ToList(); // Получаем список скидок
            return View(new Item());
        }
        
        public async Task<IActionResult> ViewItem()
        {
            var items = await _context.Items.ToListAsync(); // Получаем все товары из базы данных
            return View(items); // Возвращаем представление с товарами
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int vendorID, int discountID, string title, string description, decimal price, string date, int inventory)
        {
            ViewBag.Vendor = _context.Vendor.ToList(); // Получаем список поставщиков
            ViewBag.Discount = _context.Discount.ToList(); // Получаем список скидок

            // Проверка на валидность входных данных
            if (vendorID <= 0 || discountID <= 0 || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || price <= 0 || string.IsNullOrEmpty(date) || inventory < 0)
            {
                ModelState.AddModelError("DateDelivery", "Пожалуйста, заполните все обязательные поля.");
                var item = new Item
                {
                    VendorID = vendorID,
                    DiscountID = discountID,
                    Title = title,
                    Description = description,
                    Price = price,
                    DateDelivery = date,
                    Inventory = inventory
                };
                return View(item); // Возвращаем представление с заполненной моделью
            }

            var newItem = new Item(vendorID, discountID, title, description, price, date, inventory);

            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Товар успешно добавлен!";
            ModelState.Clear();
            return RedirectToAction("Items");
        }

        [HttpGet] // Метод для редактирования товара
        public IActionResult EditItem()
        {
            ViewBag.Vendor = _context.Vendor.ToList(); // Получаем список поставщиков
            ViewBag.Discount = _context.Discount.ToList(); // Получаем список скидок
            return View(new Item());
        }
        [HttpGet("admin/edit")]
        public async Task<IActionResult> EditItem(int id)
        {
            // Получаем товар по его идентификатору
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(); // Если товар не найден, возвращаем 404
            }

            // Получаем списки поставщиков и скидок для выпадающих списков
            ViewBag.Vendor = await _context.Vendor.ToListAsync();
            ViewBag.Discount = await _context.Discount.ToListAsync();

            return View(item); // Возвращаем представление с данными товара
        }
        [HttpPost("admin/edit")]
        public async Task<IActionResult> EditItem(int id, int vendorID, int discountID, string title, string description, decimal price, string date, int inventory)
        {
            // Получаем товар по его идентификатору
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(); // Если товар не найден, возвращаем 404
            }

            // Проверка на валидность входных данных
            if (vendorID <= 0 || discountID <= 0 || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || price <= 0 || string.IsNullOrEmpty(date) || inventory < 0)
            {
                ModelState.AddModelError("", "Пожалуйста, заполните все обязательные поля.");

                // Заполняем временный объект для возврата в представление
                var itemTemp = new Item
                {
                    Id = item.Id, // Сохраняем ID для возврата
                    VendorID = vendorID,
                    DiscountID = discountID,
                    Title = title,
                    Description = description,
                    Price = price,
                    DateDelivery = date,
                    Inventory = inventory
                };

                // Получаем списки поставщиков и скидок для выпадающих списков
                ViewBag.Vendor = await _context.Vendor.ToListAsync();
                ViewBag.Discount = await _context.Discount.ToListAsync();

                return View(itemTemp); // Возвращаем представление с заполненной моделью
            }

            // Обновляем свойства товара
            item.VendorID = vendorID;
            item.DiscountID = discountID;
            item.Title = title;
            item.Description = description;
            item.Price = price;
            item.DateDelivery = date; // Убедитесь, что это правильный формат
            item.Inventory = inventory;

            // Обновляем товар в базе данных
            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Товар успешно изменен!";
            return RedirectToAction("Items"); // Перенаправление на страницу со списком товаров
        }

        public async Task<IActionResult> DeleteItem(int id) // Метод для удаления товара
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item); // Возвращает форму подтверждения удаления
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItems = await _context.OrderItems.Where(oi => oi.ItemID == id).ToListAsync();
            if (orderItems.Any())
            {
                TempData["ErrorMessage"] = "Невозможно удалить товар, так как он связан с заказами.";
                return RedirectToAction("Items");
            }

            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Товар успешно удален.";
            return RedirectToAction("Items");
        }

       
    }
}