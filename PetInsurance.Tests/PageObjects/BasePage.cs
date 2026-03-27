using Microsoft.Playwright;

namespace PetInsurance.Tests.PageObjects
{
    public class BasePage
    {
        protected readonly IPage Page;

        public BasePage(IPage page)
        {
            Page = page;
        }

        // 🔍 Корисні методи для всіх сторінок
        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<string> GetPageTitleAsync()
        {
            return await Page.TitleAsync();
        }

        public async Task TakeScreenshotAsync(string fileName)
        {
            await Page.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = $"test-results/{fileName}",
                FullPage = true 
            });
        }
    }
}