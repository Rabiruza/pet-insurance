using Microsoft.Playwright;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlaywrightApiTests
{
    [TestFixture]
    public class InterviewPrepTests
    {
        private IAPIRequestContext _apiContext;

        [SetUp]
        public async Task Setup()
        {
            var playwright = await Playwright.CreateAsync();
            _apiContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = "https://jsonplaceholder.typicode.com"
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            await _apiContext.DisposeAsync();
        }

        [Test]
        public async Task LoginTest()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            var page = await browser.NewPageAsync();

            await page.GotoAsync("https://the-internet.herokuapp.com/login");
            LoginPageTest loginPage = new(page);
            loginPage.LoginAsync("tomsmith", "SuperSecretPassword!");
            
            var flashMessage = await page.Locator("#flash").InnerTextAsync();
            loginPage.IsLoginSuccessfulAsync(flashMessage);
        }

        // 1. Базовий GET + перевірка статусу та JSON
        [Test]
        public async Task GetPost_ShouldReturnCorrectData()
        {
            var response = await _apiContext.GetAsync("/posts/1");

            Assert.That(response.Status, Is.EqualTo(200));

            var json = await response.JsonAsync();
            var title = json?.GetProperty("title").GetString();

            Assert.That(title, Is.Not.Null.And.Not.Empty);
            Console.WriteLine($"Title: {title}");
        }

        // 2. POST з тілом + заголовки авторизації
        [Test]
        public async Task CreateUser_ShouldReturnId()
        {
            var payload = new { name = "Test", job = "QA" };

            var response = await _apiContext.PostAsync("/users", new()
            {
                DataObject = payload,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                    // { "Authorization", "Bearer your-token" }  // якщо треба
                }
            });

            Assert.That(response.Status, Is.EqualTo(201));

            var json = await response.JsonAsync();
            var id = json?.GetProperty("id").GetInt32();

            Assert.That(id, Is.GreaterThan(0));
        }

        // 3. Chaining запитів (POST → GET)
        [Test]
        public async Task CreateAndVerifyUser()
        {
            // 1. Create
            var createResponse = await _apiContext.PostAsync("/users", new()
            {
                DataObject = new { name = "Test User", job = "QA" }
            });
            Console.WriteLine(await createResponse.TextAsync());

            var created = await createResponse.JsonAsync();
            Console.WriteLine(createResponse.Status);
            Console.WriteLine(createResponse.TextAsync());

            var id = created?.GetProperty("id").GetInt32();

            // 2. Get by id
            var getResponse = await _apiContext.GetAsync($"/users/{id}");
            var user = await getResponse.JsonAsync();

            Assert.That(user?.GetProperty("name").GetString(), Is.EqualTo("Test User"));
        }

        // 4. Reusable helper (POST з обробкою помилок)
        [Test]
        public async Task PostWithHelper_ShouldWork()
        {
            var payload = new { name = "Helper Test", job = "Senior QA" };

            var user = await PostAsync<UserDto>(_apiContext, "/users", payload);

            Assert.That(user.Id, Is.GreaterThan(0));
            Console.WriteLine($"Created user with ID: {user.Id}");
        }

        [Test]
        public async Task CreateUser_FakeApi_ReturnsEcho()
        {
            var createResponse = await _apiContext.PostAsync("/users", new()
            {
                DataObject = new { name = "Test User", job = "QA" }
            });

            var created = await createResponse.JsonAsync();

            Assert.That(created?.GetProperty("name").GetString(), Is.EqualTo("Test User"));
            Assert.That(created?.GetProperty("job").GetString(), Is.EqualTo("QA"));
        }


        // Reusable helper метод
        private static async Task<T> PostAsync<T>(IAPIRequestContext context, string url, object data)
        {
            var response = await context.PostAsync(url, new() { DataObject = data });

            if (!response.Ok)
            {
                var errorBody = await response.TextAsync();
                throw new Exception($"API failed with status {response.Status}: {errorBody}");
            }

            return (await response.JsonAsync<T>())!;
        }
    }

    // Допоміжний клас для десеріалізації
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
    }

    public class LoginPageTest
    {
        private readonly IPage _page;

        public LoginPageTest(IPage page) => _page = page;

        private ILocator Username => _page.Locator("#username");
        private ILocator Password => _page.Locator("#password");
        private ILocator LoginButton => _page.Locator("button[type='submit']");
        private ILocator SuccessMessage => _page.Locator("#flash");

        public async Task LoginAsync(string username, string password)
        {
            await Username.FillAsync(username);
            await Password.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        public async Task<bool> IsLoginSuccessfulAsync(string message)
        {
            Assert.That(message, Does.Contain("You logged into a secure area!"));
            return true;
        }

        public static async Task<bool> WaitAndClickWithRetryAsync(IPage page, string selector, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await page.WaitForSelectorAsync(selector, new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
                    await page.ClickAsync(selector);
                    return true;
                }
                catch
                {
                    if (i == maxRetries - 1)
                    {
                        await page.ScreenshotAsync(new() { Path = $"error_{DateTime.Now:yyyyMMddHHmmss}.png" });
                        throw;
                    }
                    await Task.Delay(1000);
                }
            }
            return false;
        }
    }
}