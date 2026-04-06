using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

using Azure;

using FluentAssertions.Common;

using Microsoft.Azure.Amqp.Framing;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Microsoft.VisualBasic.FileIO;

using NUnit.Framework;

using SQLitePCL;

namespace PetInsurance.Tests.Tests.UI;

[TestFixture]
public class ITCraftAutomationQATests
{
    private IPage _page;
    private IAPIRequestContext _apiContext;

    [SetUp]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,

        });
        _page = await browser.NewPageAsync();
        _apiContext = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://jsonplaceholder.typicode.com"
        });

    }

    [TearDown]
    public async Task TearDown()
    {
        await _page.CloseAsync();
    }

    // ==================== UI + Table Tests ====================
    [Test]
    public async Task Playwright_Basics_Locators_Waits_Expect()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/tables");
        await _page.WaitForSelectorAsync("#table1", new() { State = WaitForSelectorState.Visible });
        var table = _page.Locator("#table1");
        Assert.That(await table.IsVisibleAsync(), Is.True, "Таблиця повинна бути видимою");
        var rowCount = await table.Locator("tbody tr").CountAsync();
        Assert.That(rowCount, Is.EqualTo(4), "Очікується 4 рядки в таблиці");
        var cells = await table.Locator("tr td").AllInnerTextsAsync();

        Assert.That(cells[14], Is.EqualTo("jdoe@hotmail.com"), "Очікується, що 14 клітинка містить jdoe@hotmail.com");
    }

    [Test]
    public async Task WorkWithTable_ExtractAndFilterData()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/tables");
        var rows = await _page.Locator("#table1 tbody tr").AllAsync();
        foreach (var row in rows)
        {
            var cells = await row.Locator("td").AllTextContentsAsync();
            Console.WriteLine($"Row: {string.Join(" | ", cells)}");
        }

        var bachRow = _page.Locator("#table1 tr").Filter(new() { HasText = "Bach" });
        Assert.That(await bachRow.IsVisibleAsync(), Is.True, "Очікується, що рядок з Bach буде видимим");
    }

    [Test]
    public async Task Playwright_Expectations()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/dynamic_loading/1");
        var startButton = _page.GetByRole(AriaRole.Button, new() { Name = "Start" });
        await startButton.ClickAsync();
        var finishText = _page.Locator("#finish h4");
        Assert.That(await finishText.InnerTextAsync(), Is.EqualTo("Hello World!"));
    }

    [Test]
    public async Task WorkWithAddRemoveElements()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/add_remove_elements/");
        var addButton = _page.GetByRole(AriaRole.Button, new() { Name = "Add Element" });
        //await _page.PauseAsync();
        Assert.That(await addButton.IsVisibleAsync(), Is.True, "Очікується, що кнопка Add Element буде видимою");
        await addButton.ClickAsync();
        var deleteButton = _page.GetByRole(AriaRole.Button, new() { Name = "Delete" });
        //await _page.PauseAsync();
        Assert.That(await deleteButton.IsVisibleAsync(), Is.True, "Очікується, що кнопка Delete буде видимою після додавання");
        await deleteButton.ClickAsync();
        Assert.That(await deleteButton.IsVisibleAsync(), Is.False, "Очікується, що кнопка Delete буде видалена після кліку");
    }

    [Test]
    public async Task WorkWith_Authentification()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/login");

        var userNameTextBox = _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
        await userNameTextBox.FillAsync("tomsmith");

        var passwordTextBox = _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
        await passwordTextBox.FillAsync("SuperSecretPassword!");

        var loginButton = _page.GetByRole(AriaRole.Button, new() { Name = " Login" });
        await loginButton.ClickAsync();

        var succesfullLoginMessage = _page.GetByText("You logged into a secure area");
        Assert.That(await succesfullLoginMessage.IsVisibleAsync(), Is.True, "Очікується, що повідомлення про успішний логін буде видимим");
    }

    [Test]
    public async Task WorkWith_DragAndDrop()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/drag_and_drop");
        var columnA = _page.Locator("#column-a");
        var columnB = _page.Locator("#column-b");
        await columnA.DragToAsync(columnB);
        //await _page.PauseAsync();
        var headerA = await columnA.Locator("header").InnerTextAsync();
        var headerB = await columnB.Locator("header").InnerTextAsync();

        Assert.That(headerA, Is.EqualTo("B"), "Очікується, що після перетягування у колонці A буде заголовок B");
        Assert.That(headerB, Is.EqualTo("A"), "Очікується, що після перетягування у колонці B буде заголовок A");
    }

    [Test]
    public async Task WorkWith_CheckBoxes()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/checkboxes");
        var checkboxes = _page.Locator("#checkboxes input[type='checkbox']");

        Assert.That(await checkboxes.CountAsync(), Is.EqualTo(2), "Очікується, що на сторінці буде 2 чекбокси");
        Assert.That(await checkboxes.Nth(0).IsCheckedAsync(), Is.False, "Очікується, що перший чекбокс буде не вибраний за замовчуванням");
        Assert.That(await checkboxes.Nth(1).IsCheckedAsync(), Is.True, "Очікується, що другий чекбокс буде вибраний за замовчуванням");

        await checkboxes.Nth(0).CheckAsync();
        await checkboxes.Nth(1).UncheckAsync();

        Assert.That(await checkboxes.Nth(0).IsCheckedAsync(), Is.True, "Очікується, що перший чекбокс буде вибраний після кліку");
        Assert.That(await checkboxes.Nth(1).IsCheckedAsync(), Is.False, "Очікується, що другий чекбокс буде не вибраний після кліку");
    }

    [Test]
    public async Task WorkWith_IFrame()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/iframe");
        var frame = _page.FrameLocator("#mce_0_ifr");
        var editor = frame.Locator("#tinymce");
        var text = await editor.InnerTextAsync();
        Assert.That(text, Is.EqualTo("Your content goes here."), "Очікується, що текст в iframe буде 'Your content goes here.'");
    }

    //Dropdown
    [Test]
    public async Task WorkWith_DropDown()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/dropdown");
        var dropDown = _page.Locator("#dropdown");
        await dropDown.SelectOptionAsync("Option 2");

        var selectedText = await dropDown.Locator("option:checked").InnerTextAsync();
        Assert.That(selectedText, Is.EqualTo("Option 2"));

        Assert.That(await dropDown.InputValueAsync(), Is.EqualTo("2"), "Очікується, що після вибору Option 2, значення dropdown буде '2'");
    }

    // Dynamic Controls
    [Test]
    public async Task WorkWith_DynamicControls()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/dynamic_controls");
        var checkBox = _page.GetByText("A checkbox");
        Assert.That(await checkBox.IsVisibleAsync(), Is.True, "Очікується, що чекбокс буде видимим на сторінці");

        var removeButton = _page.GetByRole(AriaRole.Button, new() { Name = "Remove" });
        await removeButton.ClickAsync();
        await WaitForItSelectorIsHidden();

        Assert.That(await checkBox.IsVisibleAsync(), Is.False, "Очікується, що чекбокс будет скрыт после нажатия Remove");
        var noCheckboxMessage = _page.GetByText("It's gone!");
        Assert.That(await noCheckboxMessage.IsVisibleAsync(), Is.True, "Очікується, що після удаления чекбокса появится сообщение 'It's gone!'");

        var addButton = _page.GetByRole(AriaRole.Button, new() { Name = "Add" });
        await addButton.ClickAsync();
        await WaitForItSelectorIsHidden();

        Assert.That(await checkBox.IsVisibleAsync(), Is.True, "Очікується, що чекбокс будет видимым после нажатия Add");

        var checkboxMessage = _page.GetByText("It's back!");
        Assert.That(await checkboxMessage.IsVisibleAsync(), Is.True, "Очікується, що после добавления чекбокса появится сообщение 'It's back!'");
        //await _page.PauseAsync();
    }

    [Test]
    public async Task WorkWith_JavaScriptClickAlert()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");

        _page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Is.EqualTo("I am a JS Alert"));
            await dialog.AcceptAsync();
        };

        await ClickJSConfirmAlertButton();

        Assert.That(await _page.Locator("#result").InnerTextAsync(), Is.EqualTo("You successfully clicked an alert"), "Очікується, що після принятия алерта появится результат 'You successfully clicked an alert'");
        //await _page.PauseAsync();
    }

    [Test]
    public async Task WorkWith_JavaScriptConfirmAlertClickOk()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        _page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Is.EqualTo("I am a JS Confirm"));
            await dialog.AcceptAsync();
        };

        await ClickJSConfirmAlertButton();

        Assert.That(await _page.Locator("#result").InnerTextAsync(), Is.EqualTo("You clicked: Ok"), "Очікується, що после принятия confirm алерта появится результат 'You clicked: Ok'");
    }


    [Test]
    public async Task WorkWith_JavaScriptConfirmAlertClickCancel()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        _page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Is.EqualTo("I am a JS Confirm"));
            await dialog.DismissAsync();
        };

        await ClickJSConfirmAlertButton();

        Assert.That(await _page.Locator("#result").InnerTextAsync(), Is.EqualTo("You clicked: Cancel"), "Очікується, что после отклонения confirm алерта появится результат 'You clicked: Cancel'");
    }

    [Test]
    public async Task WorkWith_JavaScriptPromptAlertText()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        _page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Is.EqualTo("I am a JS prompt"));
            await dialog.AcceptAsync("Playwright Test");
        };
        var promptButton = _page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Prompt" });
        await promptButton.ClickAsync();
        Assert.That(await _page.Locator("#result").InnerTextAsync(), Is.EqualTo("You entered: Playwright Test"), "Очікується, что после принятия prompt алерта с текстом 'Playwright Test' появится результат 'You entered: Playwright Test'");
    }

    [Test]
    public async Task WorkWith_JavaScriptPromptAlertCancel()
    {
        await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        _page.Dialog += async (_, dialog) =>
        {
            Assert.That(dialog.Message, Is.EqualTo("I am a JS prompt"));
            await dialog.DismissAsync();
        };
        var promptButton = _page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Prompt" });
        await promptButton.ClickAsync();
        Assert.That(await _page.Locator("#result").InnerTextAsync(), Is.EqualTo("You entered: null"), "Очікується, что после отклонения prompt алерта появится результат 'You entered: null'");
    }

    private async Task WaitForItSelectorIsHidden()
    {
        await _page.WaitForSelectorAsync("text=Wait for it...", new() { State = WaitForSelectorState.Hidden });
    }

    private async Task ClickJSConfirmAlertButton()
    {
        var jsConfirmButton = _page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Confirm" });
        await jsConfirmButton.ClickAsync();
    }

    // ==================== API TESTING  ====================

    // 1. Базовий GET + параметри + перевірка JSON
    [Test]
    public async Task Api_Get_WithQueryParams()
    {
        var response = await _apiContext.GetAsync("/posts", new()
        {
            Params = new Dictionary<string, object> { { "userId", 1 } }
        });

        Assert.That(response.Status, Is.EqualTo(200));
        var posts = await response.JsonAsync();
        var postCount = posts?.GetArrayLength() ?? 0;
        Assert.That(postCount, Is.GreaterThan(0));
    }

    // 2. POST + перевірка статусу та відповіді
    [Test]
    public async Task Api_Post_CreateResource()
    {
        var payload = new { title = "Test", body = "Automation in .NET", userId = 1 };

        var response = await _apiContext.PostAsync("/posts", new() { DataObject = payload });

        Assert.That(response.Status, Is.EqualTo(201));
        var json = await response.JsonAsync();
        var id = json?.GetProperty("id").GetInt32();
        Assert.That(id, Is.GreaterThan(0));
    }

    // 3. Chaining запитів (POST → GET) — Integration level
    [Test]
    public async Task Api_Chaining_Post()
    {
        // Create
        var createPayload = new { title = "Chaining Test", body = "Playwright .NET", userId = 1 };
        var createResponse = await _apiContext.PostAsync("/posts", new() { DataObject = createPayload });
        Assert.That(createResponse.Status, Is.EqualTo(201), "Очікується, що POST поверне 201 Created");

        //Якщо ти тестуватимеш справжній бекенд, тоді ланцюжок POST → GET буде коректним, бо сервер поверне id.
        //var createResponse = await _apiContext.PostAsync("/posts", new() { DataObject = createPayload });
        //var created = await createResponse.JsonAsync();
        //var id = created?.GetProperty("id").GetInt32();

        //// Get by id
        //var getResponse = await _apiContext.GetAsync($"/posts/{id}");
        //var post = await getResponse.JsonAsync();

        //Assert.That(post?.GetProperty("title").GetString(), Is.EqualTo("Chaining Test"));
    }

    // 4. Reusable Helper + Error Handling (дуже важливо для senior)
    [Test]
    public async Task Api_ReusableHelper_WithErrorHandling()
    {
        var payload = new { name = "Test Helper", job = "Automation" };

        var user = await PostWithErrorHandlingAsync<UserDto>(_apiContext, "/users", payload);

        Assert.That(user, Is.Not.Null, "Очікується, що відповідь буде десеріалізована у UserDto");

        // Перевірка Id (для реального API)
        if (user.Id > 0)
        {
            Assert.That(user.Id, Is.GreaterThan(0), "Очікується, що створений користувач матиме Id > 0");
        }
        else
        {
            Assert.Warn("API не повернув Id — це очікувано для jsonplaceholder");
        }
        Console.WriteLine($"Created user ID: {user.Id}");
    }

    [Test]
    public async Task API_UpdateAndPatchExistingResourceTest()
    {
        var payload = new { title = "Test", body = "Automation in .NET", userId = 1 };
        var response = await _apiContext.PutAsync("/posts/1", new() { DataObject = payload });
        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json?.GetProperty("title").GetString(), Is.EqualTo("Test"));
        Assert.That(json?.GetProperty("body").GetString(), Is.EqualTo("Automation in .NET"));

        var updatedPayload = new { title = "Test", body = "Updated", userId = 1 };
        var patchResponse = await _apiContext.PatchAsync("/posts/1", new() { DataObject = updatedPayload });
        Assert.That(patchResponse.Status, Is.EqualTo(200));

        var patchJson = await patchResponse.JsonAsync();
        Assert.That(patchJson?.GetProperty("title").GetString(), Is.EqualTo("Test"));
        Assert.That(patchJson?.GetProperty("body").GetString(), Is.EqualTo("Updated"));
    }

    [Test]
    public async Task API_UpdateNonExistingResourceTest()
    {
        var payload = new { title = "Test", body = "Automation in .NET", userId = 1 };
        var response = await _apiContext.PutAsync("/posts/9999", new() { DataObject = payload });
        Assert.That(response.Status, Is.EqualTo(404).Or.EqualTo(500),
    "Очікується, що при оновленні неіснуючого ресурсу сервер поверне 404 або 500");
        Console.WriteLine($"Status: {response.Status}, Body: {await response.TextAsync()}");

        //Для реального API
        //Assert.That(response.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task API_DeleteResourceTest()
    {
        var response = await _apiContext.DeleteAsync("/posts/1");
        Assert.That(response.Status, Is.EqualTo(200).Or.EqualTo(204), "Очікується, що при видаленні ресурсу сервер верне 200 або 204");

        var getResponse = await _apiContext.GetAsync("/posts/1");
        //Для реального API
        //Assert.That(getResponse.Status, Is.EqualTo(404), "Очікується, что после удаления ресурса GET запрос вернет 404");
    }

    [Test]
    public async Task API_DeleteNonExistingResourceTest()
    {
        var response = await _apiContext.DeleteAsync("/posts/9999");
        //Для реального API
        //Assert.That(response.Status, Is.EqualTo(404).Or.EqualTo(500), "Очікується, что при удалении неіснуючого ресурсу сервер вернет 404 или 500");
        Console.WriteLine($"Status: {response.Status}, Body: {await response.TextAsync()}");
    }   



    // ==================== Page Object (BDD-ready) ====================
    [Test]
    public async Task UsePageObject_BDD_Ready()
    {
        var loginPage = new LoginPage(_page);
        await loginPage.LoginAsync("tomsmith", "SuperSecretPassword!");
        await loginPage.VerifySuccessfulLoginAsync();
    }

    // ====================== Reusable API Helper ======================
    private static async Task<T> PostWithErrorHandlingAsync<T>(IAPIRequestContext context, string url, object data)
    {
        var response = await context.PostAsync(url, new() { DataObject = data });

        if (!response.Ok)
            throw new Exception($"API call failed. Status: {response.Status}. Body: {await response.TextAsync()}");

        var json = await response.JsonAsync<T>();
        if (json == null)
            throw new Exception("Response body could not be deserialized into DTO");

        return json;
    }
}



//## 🔎 Оновлений список завдань для навчання Playwright API
//1. ** PUT / PATCH тест**                              done
//   -Онови існуючий ресурс(наприклад, `/posts/1`).  
//   - Перевір статус‑код(200 OK).  
//   - Перевір, що тіло відповіді містить оновлені дані.
//   - Додай негативний сценарій: оновлення неіснуючого ресурсу → 404.

//2. **DELETE тест**                             done
//   - Видали ресурс (наприклад, `/posts/1`).  
//   - Перевір статус‑код(200 OK або 204).  
//   - Виконай GET після DELETE → очікуй 404.  
//   - Додай негативний сценарій: DELETE за неіснуючим id.

//3. **Chaining CRUD (повний цикл)**
//   - POST → отримати id (або використати фіктивний id).  
//   - GET → перевірити створені дані.
//   - PUT → оновити дані.
//   - GET → перевірити оновлення.  
//   - DELETE → видалити.
//   - GET → перевірити, що ресурс не існує.  
//   *(Це інтеграційний сценарій, який імітує реальну роботу з API.)*

//4. ** Headers &Response Time**  
//   - Перевір, що відповідь має правильний `Content-Type`.  
//   - Виміряй час відповіді(`response.Timing`) і перевір, що він< 2 сек.

//5. **Schema Validation**  
//   - Використай DTO для десеріалізації.
//   - Перевір, що всі очікувані поля присутні.
//   - Якщо поле відсутнє → кинути `Assert.Warn`.

//6. **Authorization сценарії**  
//   - Використай `_apiContext` із токеном(наприклад, Bearer).  
//   - Перевір позитивний сценарій(валідний токен).  
//   - Перевір негативний сценарій(невалідний токен → 401 Unauthorized).

//---

//## 📌 Практичні вправи
//- Напиши** PUT → GET** тест із перевіркою оновлених даних.
//- Реалізуй** DELETE → GET** тест із перевіркою, що ресурс видалений.
//- Додай** повний CRUD‑ланцюжок** як інтеграційний тест.  
//- Перевір** headers** і** response time** для будь‑якого запиту.  
//- Додай** авторизаційний тест** із валідним і невалідним токеном.  


// ====================== Page Object ======================
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

    public async Task VerifySuccessfulLoginAsync()
    {
        //await Expect(SuccessMessage).ToContainTextAsync("You logged into a secure area!");
    }
}

// DTO для десеріалізації
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
}



