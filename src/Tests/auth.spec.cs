using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
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
using UserAuthNOrg.Utilities.Enums;
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

        [Theory, AutoNSubstituteData]
        public async Task SignUp_Validation_Failure_Should_Return_StatusCode_422(SignUpDTO model)
        {
            // Arrange
            model.FirstName = null;
            model.LastName = null;
            model.Password = null;
            model.Email = "dev@sbsc.com";

            var resultModel = new ApiResponse<List<Error>>(BuildError());

            // Act
            var result = await _sut.SignUp(model);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;

            objectResult.StatusCode.Should().Be(422);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeEquivalentTo(resultModel, x => x.ComparingRecordsByValue().ComparingByMembers<ApiResponse<List<Error>>>());

            var objResultValue = (ApiResponse<List<Error>>)objectResult.Value;
            objResultValue?.Errors.Count().Should().Be(3);
        }

        [Theory, AutoNSubstituteData]
        public async Task SignUp_UserExist_Should_Return_BadRequest(SignUpDTO model)
        {
            // Arrange
            model.Email = "dev@yopmail.com";

            var user = TestData.GetUsers().FirstOrDefault();

            var resultModel = new ApiResponse<string>(
                        "Email belongs to an existing customer",
                        StatusCode.BadRequest);

            UserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(user));

            // Act
            var result = await _sut.SignUp(model);

            // Assert
            UserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = (BadRequestObjectResult)result;

            objectResult.StatusCode.Should().Be(400);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeEquivalentTo(resultModel, x => x.ComparingRecordsByValue().ComparingByMembers<ApiResponse<string>>());

            var objResultValue = (ApiResponse<string>)objectResult.Value;
            objResultValue?.Errors.Should().BeNull();
            objResultValue?.Message.Should().BeSameAs(resultModel.Message);
            objResultValue?.StatusCode.ToString().Should().BeSameAs(resultModel.StatusCode.ToString());
        }

        [Theory, AutoNSubstituteData]
        public async Task SignUp_CreatAsyncFailed_Should_Return_BadRequest(SignUpDTO model)
        {
            // Arrange
            model.Email = "dev2@yopmail.com";

            var resultModel = new ApiResponse<string>(
                        "User creation not successful!",
                        StatusCode.BadRequest);

            UserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _sut.SignUp(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = (BadRequestObjectResult)result;

            objectResult.StatusCode.Should().Be(400);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeEquivalentTo(resultModel, x => x.ComparingRecordsByValue().ComparingByMembers<ApiResponse<string>>());

            var objResultValue = (ApiResponse<string>)objectResult.Value;
            objResultValue?.Errors.Should().BeNull();
            objResultValue?.Message.Should().BeSameAs(resultModel.Message);
            objResultValue?.StatusCode.ToString().Should().BeSameAs(resultModel.StatusCode.ToString());
        }

        [Theory, AutoNSubstituteData]
        public async Task SignUp_CreatAsyncPassed_Should_Return_Ok_TokenNotNull(SignUpDTO model)
        {
            // Arrange
            model.Email = "success@yopmail.com";

            var resultModel = new ApiResponse<SuccessfulUserCreation>(
                        new SuccessfulUserCreation()
                        {
                            User = new ViewUser()
                            {
                                Email = model.Email,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Phone = model.Phone
                            }
                        },
                        "Registration successful",
                        StatusCode.Created);

            UserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            RoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            UserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
              .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.SignUp(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be(201);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.Should().BeEquivalentTo(resultModel, x => x.ComparingRecordsByValue().ComparingByMembers<ApiResponse<SuccessfulUserCreation>>());

            var objResultValue = (ApiResponse<SuccessfulUserCreation>)objectResult.Value;
            objResultValue?.Errors.Should().BeNull();
            objResultValue?.Message.Should().BeSameAs(resultModel.Message);
            objResultValue?.StatusCode.ToString().Should().BeSameAs(resultModel.StatusCode.ToString());
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
