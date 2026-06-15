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
    public class WarehouseKeeperController : Controller
    {
        private readonly ToDoListContext _context;

        public WarehouseKeeperController(ToDoListContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
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
    }
}
