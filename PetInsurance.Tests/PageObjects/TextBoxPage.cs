using Microsoft.Playwright;

namespace PetInsurance.Tests.PageObjects
{
    public class TextBoxPage : BasePage
    {
        // 🔍 Селектори - приватні, щоб не використовувати напряму в тестах
        private readonly ILocator _userNameInput;
        private readonly ILocator _userEmailInput;
        private readonly ILocator _currentAddressInput;
        private readonly ILocator _permanentAddressInput;
        private readonly ILocator _submitBtn;
        private readonly ILocator _outputName;

        public TextBoxPage(IPage page) : base(page)
        {
            _userNameInput = page.Locator("#userName");
            _userEmailInput = page.Locator("#userEmail");
            _currentAddressInput = page.Locator("#currentAddress");
            _permanentAddressInput = page.Locator("#permanentAddress");
            _submitBtn = page.Locator("#submit");
            _outputName = page.Locator("#output #name");
        }

        // 🔍 Методи-дії (бізнес-логіка, а не технічні деталі)
        // 🔍 Зверни увагу: повертаємо Task<TextBoxPage>, а не просто Task
        public async Task<TextBoxPage> FillFullNameAsync(string fullName)
        {
            await _userNameInput.FillAsync(fullName);
            return this; // 🔑 Повертаємо сам об'єкт для чейнінгу
        }

        public async Task<TextBoxPage> FillEmailAsync(string email)
        {
            await _userEmailInput.FillAsync(email);
            return this;
        }

        public async Task<TextBoxPage> FillAddressesAsync(string current, string permanent)
        {
            await _currentAddressInput.FillAsync(current);
            await _permanentAddressInput.FillAsync(permanent);
            return this;
        }

        public async Task<TextBoxPage> SubmitFormAsync()
        {
            await _submitBtn.ClickAsync();
            await WaitForPageLoadAsync();
            return this;
        }

        // 🔍 Методи-перевірки (Assertions винесені з тестів)
        public async Task<string> GetOutputNameAsync()
        {
            return await _outputName.TextContentAsync() ?? string.Empty;
        }

        public async Task<bool> IsOutputVisibleAsync()
        {
            return await _outputName.IsVisibleAsync();
        }

        // 🔍 Fluent-метод для зручного ланцюжка дій
        public async Task<TextBoxPage> FillAndSubmitAsync(
            string name, string email, string currentAddr, string permAddr)
        {
            await FillFullNameAsync(name);
            await FillEmailAsync(email);
            await FillAddressesAsync(currentAddr, permAddr);
            await SubmitFormAsync();
            return this;
        }
    }
}