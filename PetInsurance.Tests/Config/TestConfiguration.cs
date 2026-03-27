using Microsoft.Extensions.Configuration;

namespace PetInsurance.Tests.Config
{
    public class TestConfiguration
    {
        private static TestConfiguration? _instance;
        private readonly IConfiguration _config;

        private TestConfiguration()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static TestConfiguration Instance
        {
            get
            {
                _instance ??= new TestConfiguration();
                return _instance;
            }
        }

        public string BaseUrl => _config["BaseUrl"]!;
        public string ApiUrl => _config["ApiUrl"]!;
        public string PetStoreUrl => _config["PetStoreUrl"]!;
        public int Timeout => int.Parse(_config["Timeout"] ?? "30000");
        public string ConnectionString => _config["Database:ConnectionString"]!;
        public string ServiceBusConnectionString => _config["Azure:ServiceBusConnectionString"]!;
        public string ServiceBusQueueName => _config["Azure:ServiceBusQueueName"]!;
    }
}