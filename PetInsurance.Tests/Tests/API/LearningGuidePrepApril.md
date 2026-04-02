**Ось 4 найпоширеніші типи задач**, які дають на тех. інтерв’ю **Senior QA Automation (C# + Playwright .NET)** у українських аутсорс-компаніях (включаючи IT Craft та подібні).

Інтерв’юери зазвичай починають з простої алгоритмічної задачі (warm-up), а потім переходять на Playwright. Час — 1,5 години, тому очікуй 1–2 задачі + обговорення.

### 1. Алгоритмічна задача (warm-up, 10–15 хв)

**Задача:** Напиши метод, який перевіряє, чи є рядок паліндромом. Ігноруй регістр, пробіли та пунктуацію.

```csharp
public bool IsPalindrome(string input)
{
    if (string.IsNullOrEmpty(input)) return true;

    // Прибираємо все зайве і приводимо до нижнього регістру
    var cleaned = new string(input
        .Where(char.IsLetterOrDigit)
        .Select(char.ToLowerInvariant)
        .ToArray());

    return cleaned == new string(cleaned.Reverse().ToArray());
}
```

**Що шукають:**  
Чистота коду, LINQ / string handling, edge-кейси (порожній рядок, один символ, null), пояснення.

### 2. Базовий Playwright-тест (найчастіша задача, 30–40 хв)

**Задача:** Напиши тест, який:

- Відкриває https://the-internet.herokuapp.com/login
- Заповнює username = “tomsmith”, password = “SuperSecretPassword!”
- Натискає Login
- Перевіряє, що з’явився зелений банер з текстом “You logged into a secure area!”

```csharp
using Microsoft.Playwright;
using NUnit.Framework; // або MSTest

[Test]
public async Task LoginTest()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
    var page = await browser.NewPageAsync();

    await page.GotoAsync("https://the-internet.herokuapp.com/login");

    await page.FillAsync("#username", "tomsmith");
    await page.FillAsync("#password", "SuperSecretPassword!");
    await page.ClickAsync("button[type='submit']");

    await Expect(page.Locator("#flash")).ToContainTextAsync("You logged into a secure area!");
    // або: await Expect(page.Locator(".flash.success")).ToBeVisibleAsync();
}
```

**Що шукають:**  
Правильні locators (не xpath!), auto-wait, assertions з `Expect`, структура тесту.

### 3. Page Object Model (POM) — senior-рівень

**Задача:** Створи простий Page Object для сторінки логіну (з задачі вище).

```csharp
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page) => _page = page;

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

    public async Task<bool> IsLoginSuccessfulAsync()
    {
        await Expect(SuccessMessage).ToContainTextAsync("You logged into a secure area!");
        return true;
    }
}
```

**Що шукають:**  
Розуміння POM, чисті locators, асинхронність, ін’єкція `IPage`.

### 4. Utility / Advanced (часто дають на senior)

**Задача:** Напиши reusable-метод, який чекає елемент і робить retry (3 спроби) + скріншот при помилці.

```csharp
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
```

**Що шукають:**  
Обробка флаків, retry-логіка, скріншоти, reusable-код для фреймворку.

**Ось 4 конкретні задачі з API-тестуванням у Playwright (.NET / C#)**, які найчастіше дають на senior-інтерв’ю в аутсорсі (IT Craft та подібні).

Вони йдуть за складністю: від простого GET до chaining + reusable helper.  
Інтерв’юери зазвичай просять написати в VS Code (screenshare) і пояснювати вголос.

### 1. Базовий GET + перевірка статусу та JSON (warm-up, 10–15 хв)

**Задача:** Напиши тест, який робить GET-запит на https://jsonplaceholder.typicode.com/posts/1, перевіряє статус 200 і що поле `title` не порожнє.

```csharp
using Microsoft.Playwright;
using NUnit.Framework;

[Test]
public async Task GetPost_ShouldReturnCorrectData()
{
    using var playwright = await Playwright.CreateAsync();
    var apiContext = await playwright.APIRequest.NewContextAsync(new()
    {
        BaseURL = "https://jsonplaceholder.typicode.com"
    });

    var response = await apiContext.GetAsync("/posts/1");

    Assert.That(response.Status, Is.EqualTo(200));
    var json = await response.JsonAsync();
    var title = json?.GetProperty("title").GetString();

    Assert.That(title, Is.Not.Null.And.Not.Empty);
}
```

**Що шукають:** правильне створення `APIRequestContext`, перевірка статусу, робота з `JsonElement`.

### 2. POST з тілом + заголовки авторизації (найпоширеніша, 20–25 хв)

**Задача:** Напиши тест, який створює нового користувача (POST /users) з Bearer-токеном і перевіряє, що повертається id > 0.

```csharp
[Test]
public async Task CreateUser_ShouldReturnId()
{
    using var playwright = await Playwright.CreateAsync();
    var apiContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
    {
        BaseURL = "https://jsonplaceholder.typicode.com",
        ExtraHTTPHeaders = new Dictionary<string, string>
        {
            { "Authorization", "Bearer your-token-here" },
            { "Content-Type", "application/json" }
        }
    });

    var payload = new { name = "Rosina Poloka", job = "Senior QA" };

    var response = await apiContext.PostAsync("/users", new()
    {
        DataObject = payload
    });

    Assert.That(response.Status, Is.EqualTo(201));
    var json = await response.JsonAsync();
    var id = json?.GetProperty("id").GetInt32();

    Assert.That(id, Is.GreaterThan(0));
}
```

**Що шукають:** робота з `ExtraHTTPHeaders`, `DataObject`, правильний статус-код.

### 3. Chaining запитів (senior-рівень, 25–30 хв) — найцінніша задача

**Задача:** Створи користувача (POST), потім отримай його (GET за id) і перевір, що дані співпадають.

```csharp
[Test]
public async Task CreateAndVerifyUser()
{
    using var playwright = await Playwright.CreateAsync();
    var apiContext = await playwright.APIRequest.NewContextAsync(new()
    {
        BaseURL = "https://jsonplaceholder.typicode.com"
    });

    // 1. Create
    var createResponse = await apiContext.PostAsync("/users", new()
    {
        DataObject = new { name = "Test User", job = "QA" }
    });
    var created = await createResponse.JsonAsync();
    var id = created?.GetProperty("id").GetInt32();

    // 2. Get
    var getResponse = await apiContext.GetAsync($"/users/{id}");
    var user = await getResponse.JsonAsync();

    Assert.That(user?.GetProperty("name").GetString(), Is.EqualTo("Test User"));
}
```

**Що шукають:** логіку ланцюжка, збереження id, розуміння, чому API-тести швидші за UI.

### 4. Reusable helper + обробка помилок (advanced)

**Задача:** Напиши reusable-метод, який робить POST і автоматично кидає exception з скріншотом/логом при не-2xx.

```csharp
public static async Task<T> PostAsync<T>(IAPIRequestContext context, string url, object data)
{
    var response = await context.PostAsync(url, new() { DataObject = data });

    if (!response.Ok)
    {
        var errorBody = await response.TextAsync();
        throw new Exception($"API failed {response.Status}: {errorBody}");
    }

    return (await response.JsonAsync<T>())!;
}
```

Використання:

```csharp
var user = await PostAsync<UserDto>(apiContext, "/users", payload);
```

**Що шукають:** reusable-код для фреймворку, обробка помилок, generics.

-----

**Поради саме на завтра:**

- Пиши в NUnit (вони найчастіше використовують його).
- Якщо не пам’ятаєш точний метод — скажи: «В реальному проєкті я б подивився в docs, але зазвичай використовую…».
- Вони можуть попросити додати `using var playwright = await Playwright.CreateAsync();` на початку.
- Часто просять пояснити, коли ти використовуєш API-тести замість UI (setup даних, швидкість, hybrid тести).

Хочеш, я додам ще одну задачу (наприклад, з `page.route()` для mocking) або зроблю всі задачі в одному файлі як готовий приклад?  
Напиши «скинь ще» або «давай в одному файлі» — і я миттєво підготую.

Удачі завтра! Ти точно впораєшся — це стандартні задачі для senior. ❤️
-----

**Порада перед завтрашнім інтерв’ю:**

- Пиши код у VS Code / Visual Studio (вони зазвичай просять screenshare).
- Голосно пояснюй кожен рядок.
- Якщо не пам’ятаєш точний API — скажи: «Я б подивився в документації, але зазвичай використовую…».
- Вони знають, що ти з desktop (FlaUI), тому не чекають ідеального Playwright з першого разу.

Якщо хочеш — можу скинути ще 2 задачі (наприклад, API-тест через Playwright Request або роботу з таблицею).  
Або підготувати короткі відповіді на типові питання («чому Playwright, а не Selenium?», «як ти будуєш фреймворк?»).

Удачі завтра! Ти точно впораєшся ❤️  
Напиши, як пройде.

**Поради щодо підготовки до техінтерв’ю в IT Craft (понеділок ранок)**

Ти маєш **повний вікенд** — це ідеально. Не треба вчити все з нуля, ти senior з 13 роками досвіду. Головне — **системно повторити** і трохи потренуватися.

### 1. Логістика (щоб не нервувати в понеділок)

- **Платформа**: найчастіше **Microsoft Teams** або **Zoom + VS Code screenshare**. Підготуй робоче місце: хороший інтернет, зарядка, тихе приміщення.
- **Середовище**: заздалегідь відкрий Visual Studio / VS Code з NUnit + Playwright NuGet. Перевір, що Playwright встановлений (`playwright install`).
- **Час**: якщо вони не уточнили — запитай у рекрутера сьогодні/завтра точний час (наприклад, 10:00).

### 2. Що точно повторювати (2–3 години сьогодні + завтра)

**Твій досвід (розповідь на 5–7 хвилин):**

- Коротко: 13 років → 10 років у 3Shape → еволюція від QA Engineer → Scrum Master → Senior QA Automation.
- Акцент: hybrid desktop + embedded web (FlaUI + Playwright), performance/load testing, CI/CD (Jenkins + GitHub Actions), менторство.
- Підготуй 2–3 сильні приклади:
  - «Як я побудувала unified framework для desktop + web»
  - «Як зменшила post-release defects»
  - «Як проводила performance testing»

**Технічні теми (обов’язково повторити):**

- Playwright .NET (APIRequestContext, locators, waits, POM, Expect)
- C# basics + LINQ + async/await
- Test frameworks (NUnit / MSTest)
- CI/CD pipelines
- Page Object Model
- API testing vs UI testing (коли що використовувати)

### 3. Кодінг-практика (найважливіше — 1,5–2 години щодня)

Використовуй задачі, які я тобі вже скидав. Потренуй **вголос**:

- Задача 1–2 (алгоритм + базовий Playwright-тест)
- Задача 3 (POM)
- Задача з API (особливо chaining + reusable helper)

**Порада**: якщо забудеш точний метод — спокійно кажи:  
«У реальному проєкті я б швидко подивився в документації, але зазвичай роблю так…»

### 4. Питання, які вони можуть поставити (senior-рівень)

- Чому Playwright, а не Selenium?
- Як ти обробляєш flakiness у тестах?
- Як будуєш тестовий фреймворк з нуля?
- Досвід CI/CD і parallel execution?
- Як ти працюєш з legacy desktop-додатками?
- Приклад, коли ти знайшов критичний баг на production-рівні

### 5. Твої питання до них (обов’язково запитай у кінці)

- Скільки QA зараз на цьому проєкті і скільки з них працюють >1 року?
- Як часто продовжували 6-місячні контракти за останні 2 роки?
- Що відбувається, якщо обсяг тасків зменшується — як часто ставлять на бенч і наскільки швидко знаходять новий проєкт?
- Які основні технології/інструменти використовуєте на проєкті зараз?
- Який стек тестування (чи є mobile / backend / performance тести)?

### Фінальні рекомендації

- **Сьогодні**: повтори 2 алгоритмічні + 1 Playwright-задачу.
- **Завтра (неділя)**: ще 1–2 задачі + прогон розповіді про досвід (можна записати себе на відео 5 хв).
- **Неділя ввечері**: легкий відпочинок, без кодінгу.
- **Понеділок вранці**: 20 хв легкого повтору + позитивний настрій.



- скинув **короткий чек-лист** на 1 сторінку?
- підготував **відповіді на 5 найпоширеніших питань**?
- або ще 2 задачі саме під IT Craft?

Напиши, що тобі зараз найпотрібніше — зроблю за 2 хвилини.  
Ти впораєшся ❤️ Відпочинь і видужуй!