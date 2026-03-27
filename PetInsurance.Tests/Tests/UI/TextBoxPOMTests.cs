using NUnit.Framework;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.PageObjects;
using FluentAssertions;

namespace PetInsurance.Tests.Tests.UI
{
    [TestFixture]
    public class TextBoxPOMTests : BaseUITest
    {
        private TextBoxPage _textBoxPage = null!;

        [SetUp]
        public async Task SetupPage()
        {
            await Page.GotoAsync($"{BaseUrl}/text-box");
            _textBoxPage = new TextBoxPage(Page);
            await _textBoxPage.WaitForPageLoadAsync();
        }

        [Test]
        public async Task SubmitTextBoxForm_Success()
        {
            // Arrange
            var testData = new
            {
                Name = "Anna Shevchenko",
                Email = "anna@test.com",
                CurrentAddress = "Kyiv, Ukraine",
                PermanentAddress = "Lviv, Ukraine"
            };

            // Act - тест читається як бізнес-сценарій!
            await _textBoxPage.FillAndSubmitAsync(
                testData.Name, 
                testData.Email, 
                testData.CurrentAddress, 
                testData.PermanentAddress);

            // Assert
            (await _textBoxPage.IsOutputVisibleAsync()).Should().BeTrue();
            (await _textBoxPage.GetOutputNameAsync()).Should().Contain(testData.Name);
        }

        [Test]
        public async Task SubmitTextBoxForm_WithEmptyEmail_ShouldStillSubmit()
        {
            // Arrange & Act - кожен крок окремо, з await
            await _textBoxPage.FillFullNameAsync("Test User");
            await _textBoxPage.FillAddressesAsync("Address 1", "Address 2");
            await _textBoxPage.SubmitFormAsync();

            // Assert
            (await _textBoxPage.GetOutputNameAsync()).Should().Contain("Test User");
        }
    }
}