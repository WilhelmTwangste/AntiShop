using Microsoft.AspNetCore.Mvc;
using AntiShop.Models;
using System.IO;
using ClosedXML.Excel; // Пространство имен для ClosedXML
using ToDoList.Domain;

namespace AntiShop.Controllers
{
    public class ReportController : Controller
    {
        private readonly ToDoListContext _context;

        public ReportController(ToDoListContext context)
        {
            _context = context;
        }

        public IActionResult Reports()
        {
            var orders = _context.Order.Select(o => new OrderTwoViewModel
            {
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                NameOrder = o.NameOrder
            }).ToList();

            ViewBag.TotalIncome = orders.Sum(o => o.TotalAmount);
            return View(orders);
        }

        [HttpPost]
        public IActionResult DownloadExcel()
        {
            var orders = _context.Order.Select(o => new OrderTwoViewModel
            {
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                NameOrder = o.NameOrder ?? "Без названия"
            }).ToList();

            var totalIncome = orders.Sum(o => o.TotalAmount);

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет по заказам");

                    // Заголовок
                    worksheet.Cell(1, 1).Value = "Отчет по заказам";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 3).Merge(); // Объединяем ячейки для заголовка
                    worksheet.Row(1).Height = 30;

                    // Заголовки таблицы
                    worksheet.Cell(2, 1).Value = "Дата заказа";
                    worksheet.Cell(2, 2).Value = "Название заказа";
                    worksheet.Cell(2, 3).Value = "Общая сумма";
                    worksheet.Range(2, 1, 2, 3).Style.Font.Bold = true;
                    worksheet.Range(2, 1, 2, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Данные
                    int row = 3;
                    foreach (var order in orders)
                    {
                        worksheet.Cell(row, 1).Value = order.OrderDate;
                        worksheet.Cell(row, 2).Value = order.NameOrder;
                        worksheet.Cell(row, 3).Value = order.TotalAmount;
                        row++;
                    }

                    // Итоговая сумма
                    worksheet.Cell(row, 2).Value = "Итоговая сумма продаж:";
                    worksheet.Cell(row, 3).Value = totalIncome;
                    worksheet.Range(row, 2, row, 3).Style.Font.Bold = true;

                    // Автоподбор ширины столбцов
                    worksheet.Columns().AdjustToContents();

                    // Сохранение в поток
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации Excel: {ex.Message}");
                Console.WriteLine($"Полный стек вызовов: {ex.ToString()}");
                return StatusCode(500, $"Произошла ошибка при генерации Excel: {ex.Message}");
            }
        }
    }
}