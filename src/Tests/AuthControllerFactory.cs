using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using UserAuthNOrg.Api.Controllers;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Infrastructure.Interfaces;
namespace Tests
{
    public class AuthControllerFactory
    {
        public Mock<IUserClaimsPrincipalFactory<User>> UserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        public Mock<UserManager<User>> UserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
          null, null, null, null, null, null, null, null);
        public SignInManager<User> SignInManager;
        public Mock<RoleManager<IdentityRole>> RoleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
        public Mock<IContextAccessor> ContextAccessor = new Mock<IContextAccessor>();
        public Mock<IConfiguration> Configuration = new Mock<IConfiguration>();
        public Mock<IHttpContextAccessor> HttpContextAccessor = new Mock<IHttpContextAccessor>();
        private readonly DatabaseFixture _fixture;

        public AuthControllerFactory()
        {
            SignInManager = new SignInManager<User>(
                UserManager.Object,
                HttpContextAccessor.Object,
                UserPrincipalFactory.Object,
                null,
                null,
                null,
                null);

            _fixture = new DatabaseFixture();

            AuthController = new AuthController(
                UserManager.Object,
                SignInManager,
                RoleManager.Object,
                Configuration.Object,
                ContextAccessor.Object,
                _fixture.DatabaseContext);

            AuthController.ControllerContext.HttpContext =
                new DefaultHttpContext { User = TestData.GetAuthenticatedUser() };
        }

        public AuthController AuthController { get; set; }
    }
}
