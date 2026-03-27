using Microsoft.Playwright;

using PetInsurance.Tests.Config;

namespace PetInsurance.Tests.Core
{
    public class BaseUITest
    {
        protected IPlaywright PlaywrightInstance { get; private set; } = null!;
        protected IBrowser Browser { get; private set; } = null!;
        protected IPage Page { get; private set; } = null!;
        protected string BaseUrl { get; private set; } = null!;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            PlaywrightInstance = await Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new()
            {
                Headless = false, // Зміни на false, щоб бачити браузер
                SlowMo = 50 // Сповільнення для дебагу (опціонально)
            });
            BaseUrl = TestConfiguration.Instance.BaseUrl;
        }

        [SetUp]
        public async Task Setup()
        {
            Page = await Browser.NewPageAsync();
            await Page.GotoAsync(BaseUrl);
        }

        [TearDown]
        public async Task Teardown()
        {
            await Page.CloseAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await Browser.CloseAsync();
            PlaywrightInstance.Dispose();
        }
    }
}