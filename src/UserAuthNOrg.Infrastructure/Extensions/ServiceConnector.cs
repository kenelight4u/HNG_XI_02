using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAuthNOrg.Infrastructure.Interfaces;
using UserAuthNOrg.Infrastructure.Services;

namespace UserAuthNOrg.Infrastructure.Extensions
{
    public static class ServiceConnector
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IContextAccessor, ContextAccessor>();
            services.AddScoped<IOrganizationServices, OrganizationServices>();
            services.AddScoped<IUsersServices, UsersServices>();
        }
    }
}
