using Microsoft.Extensions.Configuration;

namespace Tests
{
    public class ConfigurationHelper
    {
        public static IConfigurationRoot Configuration => new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("testappsettings.json", false, false)
           .AddEnvironmentVariables()
           .Build();
    }
}
