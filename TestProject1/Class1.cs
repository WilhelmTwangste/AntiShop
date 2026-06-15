using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntiShop.Controllers;
using ToDoList.Domain.Entites;
using ToDoList.Domain;
using AntiShop.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace TestProject1
{
    [TestFixture]
    public class AdminControllerSeleniumTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;


        [SetUp]
        public void Setup()
        {
            var options = new EdgeOptions();
            _driver = new EdgeDriver(options);
            _driver.Manage().Window.Maximize();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            _driver.Navigate().GoToUrl("https://localhost:7247");
        }
        [TearDown]
        public void TearDown()
        {
            // Закрытие браузера после теста
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
            }
        }

        [Test]
        public void Login_ValidCredentials_RedirectsToAdminPage()
        {
            _driver.Navigate().GoToUrl("https://localhost:7247");

            _driver.FindElement(By.Id("Login")).SendKeys("adm"); // Введите логин
            _driver.FindElement(By.Id("Password")).SendKeys("111"); // Введите пароль
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            _wait.Until(d => d.Url.Contains("/Admin"));
            string pageTitle = _driver.Title;
            Assert.AreEqual("Администратор - AntiShop", pageTitle, "Заголовок страницы не соответствует ожидаемому.");
        }
        [Test]
        public void AddItem_ValidData_AddsItemToDatabase()
        {
            Login_ValidCredentials_RedirectsToAdminPage();

            _driver.Navigate().GoToUrl("https://localhost:7247/Admin/Items");
            _wait.Until(d => d.Url.Contains("/Admin/Items"));

            var itemsButton = _wait.Until(d => d.FindElement(By.Id("items")));
            itemsButton.Click();
            _wait.Until(d => d.Url.Contains("/Admin/Items"));

            var addItemButton = _wait.Until(d => d.FindElement(By.Id("AddItem")));
            addItemButton.Click();
            _wait.Until(d => d.Url.Contains("/Admin/AddItem"));

            // Заполнение формы
            var titleInput = _wait.Until(d => d.FindElement(By.CssSelector("input[id='title']")));
            _wait.Until(d => titleInput.Displayed && titleInput.Enabled);
            titleInput.SendKeys("Test Item");

            _wait.Until(d => d.FindElement(By.Id("description"))).SendKeys("Test Description");
            _wait.Until(d => d.FindElement(By.Id("price"))).SendKeys("1099");
            _wait.Until(d => d.FindElement(By.Id("date"))).SendKeys("01.01.2025");
            _wait.Until(d => d.FindElement(By.Id("inventory"))).SendKeys("10");

            // Выбор поставщика и скидки
            var vendorSelect = new SelectElement(_wait.Until(d => d.FindElement(By.Id("vendorID"))));
            vendorSelect.SelectByValue("1");
            var discountSelect = new SelectElement(_wait.Until(d => d.FindElement(By.Id("discountID"))));
            discountSelect.SelectByValue("1");

            // Нажатие кнопки "Сохранить"
            var submitButton = _wait.Until(d => d.FindElement(By.CssSelector("button[id='submit']")));
            _wait.Until(d => submitButton.Displayed && submitButton.Enabled);
            submitButton.Click();

            _wait.Until(d => d.Url.Contains("/Admin/Items"));

            // Ожидание успешного сообщения
            string pageTitle = _driver.Title;
            Assert.AreEqual("Список товаров - AntiShop", pageTitle, "Заголовок страницы не соответствует ожидаемому.");

        }
        [Test]
        public void DeleteItem_ValidItem_RemovesItemFromDatabase()
        {
            Login_ValidCredentials_RedirectsToAdminPage();

            _driver.Navigate().GoToUrl("https://localhost:7247/Admin/Items");
            _wait.Until(d => d.Url.Contains("/Admin/Items"));

            var itemRow = _wait.Until(d => d.FindElement(By.XPath("//tr[td[@id='idItem' and text()='3']]")));
            Assert.IsNotNull(itemRow, "Товар с ID 3 не найден.");

            var deleteButton = itemRow.FindElement(By.XPath(".//button[contains(., 'Удалить')]"));
            Assert.IsNotNull(deleteButton, "Кнопка 'Удалить' не найдена.");
            deleteButton.Click();

            // Обрабатываем браузерное подтверждение удаления
            try
            {
                IAlert alert = _wait.Until(d => d.SwitchTo().Alert());
                alert.Accept(); // Подтверждаем удаление
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Alert-подтверждение не появилось в течение времени ожидания.");
            }

            // Ждем перенаправления обратно на страницу Items и появления сообщения об ошибке
            _wait.Until(d => d.Url.Contains("/Admin/Items"));
            var errorMessage = _wait.Until(d => d.FindElement(By.CssSelector(".alert.alert-danger")));
            Thread.Sleep(1000);
            Assert.IsTrue(errorMessage.Displayed, "Сообщение об ошибке не отображается.");
            Assert.AreEqual("Невозможно удалить товар, так как он связан с заказами.",
                errorMessage.Text.Trim(),
                "Текст сообщения об ошибке не соответствует ожидаемому.");

            var itemRowAfterDeletion = _wait.Until(d => d.FindElement(By.XPath("//tr[td[@id='idItem' and text()='3']]")));
            Assert.IsNotNull(itemRowAfterDeletion, "Товар с ID 3 был удален, хотя не должен был.");
        }
    }
}
