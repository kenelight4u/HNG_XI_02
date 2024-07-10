using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAuthNOrg.Api.Controllers;
using UserAuthNOrg.Core.Models;
using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Infrastructure.Interfaces;
using UserAuthNOrg.Utilities.Extensions;

namespace Tests
{
    public class authspec
    {
        private readonly AuthController _sut;
        public Mock<IUserClaimsPrincipalFactory<User>> UserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        public Mock<UserManager<User>> UserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
          null, null, null, null, null, null, null, null);
        public SignInManager<User> SignInManager;
        public Mock<RoleManager<IdentityRole>> RoleManager = new Mock<RoleManager<IdentityRole>>(new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
        public Mock<IContextAccessor> ContextAccessor = new Mock<IContextAccessor>();
        public Mock<IConfiguration> Configuration = new Mock<IConfiguration>();
        public Mock<IHttpContextAccessor> HttpContextAccessor = new Mock<IHttpContextAccessor>();
        private readonly DatabaseFixture _fixture;
        
        public authspec()
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

            _sut = new AuthController(
                UserManager.Object,
                SignInManager,
                RoleManager.Object,
                Configuration.Object,
                ContextAccessor.Object,
                _fixture.DatabaseContext);

            _sut.ControllerContext.HttpContext =
                new DefaultHttpContext { User = TestData.GetAuthenticatedUser() };
        }

        [Fact]
        public async Task SignUp_Validation_Failure()
        {
            // Arrange
            var model = new SignUpDTO()
            { 
                Email = "dev@sbsc.com"
            };

            var resultModel = new ApiResponse<List<Error>>(BuildError());

            // Act
            var result = await _sut.SignUp(model) as OkObjectResult;

            // Assert
            result.StatusCode.Should().Be(422);
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(resultModel, x => x.ComparingRecordsByValue().ComparingByMembers<ApiResponse<List<Error>>>());
            result.Value.Should().BeSameAs(resultModel);
        }

        private static List<Error> BuildError()
        {
            return new List<Error>()
            {
                new()
                {
                    Field = "FirstName",
                    Message = "First Name cannot be null!"
                },
                new()
                {
                    Field = "LastName",
                    Message = "Last Name cannot be null!"
                },
                new()
                {
                    Field = "Password",
                    Message = "Password cannot be null!"
                }
            };
        }
    }
}
