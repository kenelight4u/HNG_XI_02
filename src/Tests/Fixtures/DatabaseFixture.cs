using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using UserAuthNOrg.Core.Context;

namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly Guid TenantId;
        protected readonly UserAuthNOrgDbContext Database;
        private readonly string DbLocation;

        public DatabaseFixture()
        {
            DbLocation = ConfigurationHelper.Configuration["DatabasePath"];
            var connectionString = string.Format("Data Source={0}", Path.Combine(DbLocation, "TestDb"));
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            TenantId = Guid.NewGuid();

            var options = new DbContextOptionsBuilder<UserAuthNOrgDbContext>().UseNpgsql(_connection).Options;
            
            Database.Database.EnsureCreated();
            TestContext = new TestContext()
            {
                CurrentUser = Database.Users.Any() ? Database.Users.AsNoTracking().First() : DatabaseSeeder.SeedUserWithPerson(Database),
            };
        }

        public TestContext TestContext { get; }

        public UserAuthNOrgDbContext DatabaseContext => Database;

        public void Dispose()
        {
            Database.Dispose();
            _connection.Close();
        }
    }
}
