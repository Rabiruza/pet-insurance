using RestSharp;
using PetInsurance.Tests.Config;

namespace PetInsurance.Tests.Core
{
    [SetUpFixture]
    public class BaseApiTest
    {
        protected RestClient _client;
        protected string _baseUrl;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Читаємо URL з конфігурації
            _baseUrl = TestConfiguration.Instance.ApiUrl;
            
            // Налаштовуємо клієнт
            var options = new RestClientOptions(_baseUrl)
            {
                Timeout = TimeSpan.FromMilliseconds(TestConfiguration.Instance.Timeout)
            };
            _client = new RestClient(options);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
        }
    }
}