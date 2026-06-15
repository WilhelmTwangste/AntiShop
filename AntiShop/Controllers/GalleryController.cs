using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entites;
using ToDoList.Domain;
using System.Data;
using AntiShop.Models;
namespace AntiShop.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ToDoListContext _context;
        public GalleryController(ToDoListContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? itemId)
        {
            // Получаем список всех товаров для выпадающего списка
            ViewBag.Items = _context.Items.ToList();

            // Если itemId не указан, возвращаем пустой список изображений
            if (itemId == null)
            {
                return View(new List<Image>());
            }

            // Получаем изображения для выбранного товара
            var images = _context.Image.Where(i => i.ItemId == itemId).ToList();
            ViewBag.ItemId = itemId;

            return View(images);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(int itemId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var image = new Image(itemId, memoryStream.ToArray());
                    _context.Image.Add(image);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", new { itemId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id, int itemId)
        {
            var image = await _context.Image.FindAsync(id);
            if (image != null)
            {
                _context.Image.Remove(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { itemId });
        }
    }
}
