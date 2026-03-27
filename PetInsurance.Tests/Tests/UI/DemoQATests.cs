using FluentAssertions;

using Microsoft.Playwright;

using NUnit.Framework;

using PetInsurance.Tests.Core;

namespace PetInsurance.Tests.Tests.UI
{
    [TestFixture]
    public class DemoQATests : BaseUITest
    {
        [Test]
        public async Task HomePage_LoadsSuccessfully()
        {
            // Arrange - вже в BaseUITest.Page.GotoAsync()

            // Act + Assert: перевіряємо, що сторінка завантажилась
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var title = await Page.TitleAsync();
            TestContext.Out.WriteLine($"Current page title: {title}");
           
            title.Should().Contain("demosite");

            // Перевіряємо, що є хоча б один елемент з класом
            var elements = await Page.Locator(".card").AllAsync();
            elements.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TextBox_FillAndVerify()
        {
            // Arrange
            await Page.GotoAsync($"{BaseUrl}/text-box");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Act
            await Page.Locator("#userName").FillAsync("Anna Shevchenko");
            await Page.Locator("#userEmail").FillAsync("anna@test.com");
            await Page.Locator("#currentAddress").FillAsync("Kyiv, Ukraine");
            await Page.Locator("#permanentAddress").FillAsync("Lviv, Ukraine");

            await Page.Locator("#submit").ClickAsync();

            // Assert
            var output = await Page.Locator("#output #name").TextContentAsync();
            output.Should().Contain("Anna Shevchenko");
        }

        [Test]
        public async Task CheckBox_CheckAndVerify()
        {
            // Arrange
            await Page.GotoAsync($"{BaseUrl}/checkbox");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // 🔍 Крок 1: Розкриваємо дерево (якщо потрібно)
            var expandButton = Page.Locator(".rc-tree-switcher").First;
            await expandButton.ClickAsync();
            await Page.WaitForTimeoutAsync(500); // Чекаємо анімацію

            // 🔍 Крок 2: Знаходимо чекбокс Desktop простішим способом
            // Шукаємо span з текстом "Desktop", потім знаходимо чекбокс поруч
            var desktopLabel = Page.Locator("text=Desktop").First;
            await desktopLabel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            // Знаходимо чекбокс - він є батьківським або сусіднім елементом
            var desktopCheckbox = desktopLabel.Locator("xpath=./ancestor::span[contains(@class,'rc-tree-node-content-wrapper')]//preceding-sibling::span[contains(@class,'rc-tree-checkbox')]").First;

            // 🔍 Крок 3: Чекаємо на видимість і клікаємо
            await desktopCheckbox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await desktopCheckbox.ClickAsync(new LocatorClickOptions { Timeout = 5000 });

            await Page.WaitForTimeoutAsync(300); // Чекаємо оновлення UI

            // Assert – перевіряємо результат
            var result = Page.Locator("#result");
            await Assertions.Expect(result).ToBeVisibleAsync();
            await Assertions.Expect(result).ToContainTextAsync("desktop");
        }

    }
}