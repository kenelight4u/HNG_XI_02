using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserAuthNOrg.Core.Models;

namespace UserAuthNOrg.Core.Context
{
    public class UserAuthNOrgDbContext : IdentityDbContext<User>
    {
        public UserAuthNOrgDbContext(DbContextOptions<UserAuthNOrgDbContext> options) : base(options)
        {
            //comment out if you want to run migration on app start up
            //this.Database.Migrate();
        }

        public DbSet<User> AuthOrgUsers { get; set; }
        
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<UserOrganization> UserOrganization { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
