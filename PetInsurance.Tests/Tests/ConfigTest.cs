using NUnit.Framework;
using PetInsurance.Tests.Config;
using FluentAssertions;

namespace PetInsurance.Tests.Tests
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void Configuration_LoadsSuccessfully()
        {
            // Act
            var config = TestConfiguration.Instance;

            // Assert
            config.Should().NotBeNull();
            config.BaseUrl.Should().NotBeNullOrEmpty();
            config.ApiUrl.Should().NotBeNullOrEmpty();
            config.BaseUrl.Should().Be("https://demoqa.com");
        }
    }
}