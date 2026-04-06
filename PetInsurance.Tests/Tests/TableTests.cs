using Microsoft.Playwright;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaywrightTableTests
{
    [TestFixture]
    public class TableTests
    {
        private IPage _page;

        [SetUp]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            _page = await browser.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _page.CloseAsync();
        }

        // 1. Locators + Waits (з нуля)
        [Test]
        public async Task LocateAndWaitForElement()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/tables");

            // Явний wait + locator
            await _page.WaitForSelectorAsync("#table1", new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            var table = _page.Locator("#table1");                    // CSS locator
            var lastNameCell = table.Locator("tr td:nth-child(1)"); // nth-child locator

            // Перевірка видимості через NUnit
            var isVisible = await lastNameCell.IsVisibleAsync();
            Assert.That(isVisible, Is.True, "Очікується, що перша колонка таблиці буде видимою");

            Console.WriteLine("Таблиця завантажена і елемент видимий");
        }


        // 2. Робота з таблицею (найпоширеніша задача на інтерв'ю)
        [Test]
        public async Task WorkWithTable()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/tables");

            var table1 = _page.Locator("#table1");

            // Отримати всі рядки
            var rows = await table1.Locator("tbody tr").AllAsync();

            Console.WriteLine($"Кількість рядків: {rows.Count}");

            // Отримати дані з конкретного рядка (наприклад, 2-й рядок)
            var secondRow = rows[1];
            var cells = await secondRow.Locator("td").AllTextContentsAsync();

            Console.WriteLine("Дані 2-го рядка: " + string.Join(" | ", cells));

            // Знайти рядок, де Last Name = "Bach"
            var bachRow = table1.Locator("tr").Filter(new() { HasText = "Bach" });
            Assert.That(bachRow.IsVisibleAsync, Is.True);
            Console.WriteLine("Знайдено рядок з Bach");
        }

        // 3. Page Object Model (POM) — senior рівень
        [Test]
        public async Task UsePageObjectForTable()
        {
            var tablesPage = new TablesPage(_page);
            await tablesPage.GotoAsync();

            var employees = await tablesPage.GetAllEmployeesAsync();

            foreach (var emp in employees)
            {
                Console.WriteLine($"{emp.LastName}, {emp.FirstName} — {emp.Email}");
            }
            Assert.That(tablesPage.GetRowByLastName("Bach").IsVisibleAsync, Is.True);
        }

        // 4. Expect assertions (сучасний стиль Playwright)
        //[Test]
        //public async Task UseExpectAssertions()
        //{
        //    await _page.GotoAsync("https://the-internet.herokuapp.com/tables");

        //    var table = _page.Locator("#table1");

        //    // Перевірка, що таблиця має більше 3 рядків
            

        //    await Expect(table.Locator("tbody tr")).ToHaveCountAsync(4);

        //    // Перевірка тексту в комірці
        //    await Expect(table.Locator("tr td").Nth(5)).ToHaveTextAsync("jdoe@hotmail.com");

        //    // Перевірка сортування (якщо клікнути по заголовку)
        //    await table.Locator("th").Filter(new() { HasText = "Last Name" }).ClickAsync();
        //    await Expect(table.Locator("tbody tr").First()).ToContainTextAsync("Bach");
        //}

        //// 5. Бонус: Комбінація APIRequestContext + UI (дуже часто питають)
        //[Test]
        //public async Task CombineApiAndUi()
        //{
        //    // Спочатку API
        //    var apiContext = await _page.Context.APIRequest.NewContextAsync(new()
        //    {
        //        BaseURL = "https://jsonplaceholder.typicode.com"
        //    });

        //    var response = await apiContext.GetAsync("/posts/1");
        //    var json = await response.JsonAsync();

        //    // Потім UI
        //    await _page.GotoAsync("https://the-internet.herokuapp.com/tables");
        //    await Expect(_page.Locator("#table1")).ToBeVisibleAsync();

        //    Console.WriteLine("API + UI успішно комбіновано");
        //}
    }

    // ====================== Page Object Model ======================
    public class TablesPage
    {
        private readonly IPage _page;

        public TablesPage(IPage page) => _page = page;

        private ILocator Table1 => _page.Locator("#table1");

        public async Task GotoAsync() => await _page.GotoAsync("https://the-internet.herokuapp.com/tables");

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var rows = await Table1.Locator("tbody tr").AllAsync();
            var employees = new List<Employee>();

            foreach (var row in rows)
            {
                var cells = await row.Locator("td").AllTextContentsAsync();
                employees.Add(new Employee
                {
                    LastName = cells[0],
                    FirstName = cells[1],
                    Email = cells[2]
                });
            }
            return employees;
        }

        public ILocator GetRowByLastName(string lastName)
            => Table1.Locator("tr").Filter(new() { HasText = lastName });
    }

    public class Employee
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}