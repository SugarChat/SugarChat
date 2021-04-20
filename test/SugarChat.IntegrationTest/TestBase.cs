using Microsoft.Extensions.Configuration;

namespace SugarChat.IntegrationTest
{
    public abstract class TestBase
    {
        protected readonly IConfiguration _configuration;
        
        protected TestBase()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }
}