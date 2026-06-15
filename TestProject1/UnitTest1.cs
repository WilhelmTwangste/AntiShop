using Microsoft.AspNetCore.Mvc;
using AntiShop.Controllers;
using ToDoList.Domain.Entites;
using ToDoList.Domain;
using AntiShop.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TestProject1
{
    [TestFixture]
    public class AdminControllerTests
    {
        private ToDoListContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _dbContext = GetInMemoryDbContext();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        private ToDoListContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ToDoListContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new ToDoListContext(options);
        }

        [Test]
        public async Task AddItem()
        {
            var controller = new AdminController(_dbContext);

            var result = await controller.AddItem(
                vendorID: 1,
                discountID: 1,
                title: "Test Item",
                description: "Test Description",
                price: 100.0m,
                date: "2023-10-01",
                inventory: 10
            );

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result); //Проверка результат — это перенаправление
            Assert.AreEqual(1, _dbContext.Items.Count()); //Проверка, товар добавлен в базу данных
        }
        [Test]
        public async Task EditItem_InvalidData()
        {
            var vendor = new Vendor { Id = 1, NameClient = "Vendor1", VendorDate = "2023-01-01", VendorPrice = 100.0m, Commission = 10.0m };
            var discount = new Discount { Id = 1, Description = "Test Discount", Percents = 10.0m };
            _dbContext.Vendor.Add(vendor);
            _dbContext.Discount.Add(discount);
            await _dbContext.SaveChangesAsync();

            var item = new Item
            {
                VendorID = 1,
                DiscountID = 1,
                Title = "Test Item",
                Description = "Test Description",
                Price = 100.0m,
                DateDelivery = "2023-10-01",
                Inventory = 10
            };
            _dbContext.Items.Add(item);
            await _dbContext.SaveChangesAsync();

            var controller = new AdminController(_dbContext);

            //Попытка обновить товар с невалидными данными
            var result = await controller.EditItem(
                item.Id,
                vendorID: 1,
                discountID: 1,
                title: "", // Пустое название
                description: "Updated Test Description",
                price: -100.0m, // Отрицательная цена
                date: "2023-12-31",
                inventory: 20
            );

            Assert.IsInstanceOf<ViewResult>(result); // Проверяем, что результат — это ViewResult
            Assert.IsTrue(controller.ModelState.ErrorCount > 0); //Проверка, что вылетела ошибка валидации
        }
    }
}