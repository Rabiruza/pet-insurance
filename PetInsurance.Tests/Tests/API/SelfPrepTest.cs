
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Microsoft.Playwright;


namespace PetInsurance.Tests.Tests.API
{
    [TestFixture]
    public class SelfPrepTest
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
        public async Task Dispose()
        {
            await _apiContext.DisposeAsync();
        } 

        [Test]
        public async Task ShouldGet_StatusOkAndTitleNotNull()
        {
            //**Задача:** Напиши тест, який робить GET-запит на https://jsonplaceholder.typicode.com/posts/1,
            //  перевіряє статус 200 і що поле `title` не порожнє.
            var response = await _apiContext.GetAsync("/posts/1"); 
            Assert.That(response.Status, Is.EqualTo(200));

            var json = await response.JsonAsync();
            var title = json?.GetProperty("title").GetString();
            
            Assert.That(title, Is.Not.Empty); 
        }


    }
}
