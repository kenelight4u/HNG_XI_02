using UserAuthNOrg.Core.Context;
using UserAuthNOrg.Core.Models;

namespace Tests.Helpers
{
    internal static class DatabaseSeeder
    {
        public static User SeedUserWithPerson(UserAuthNOrgDbContext context)
        {
            var transaction = context.Database.BeginTransaction();
            
            var users = TestData.GetUsers();
            var organizations = TestData.GetOrganizations();
            var usersOrganization = TestData.GetUsersOrganization();
           
            context.AddRange(users);
            context.SaveChanges();

            context.AddRange(organizations);
            context.SaveChanges();

            context.AddRange(usersOrganization);
            context.SaveChanges();

            transaction.Commit();

            return users.FirstOrDefault();
        }
    }
}
