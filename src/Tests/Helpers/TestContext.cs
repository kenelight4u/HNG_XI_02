using Microsoft.Extensions.Configuration;
using UserAuthNOrg.Core.Models;

namespace Tests.Helpers
{
    public sealed class TestContext
    {
        public User CurrentUser { get; set; }

        public IConfigurationRoot Configuration => ConfigurationHelper.Configuration;
    }
}
