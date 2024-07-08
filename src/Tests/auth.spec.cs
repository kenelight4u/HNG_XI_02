using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Utilities.Extensions;

namespace Tests
{
    public class authspec
    {
        private readonly AuthControllerFactory _fac;

        public authspec()
        {
            _fac = new AuthControllerFactory();
        }

        [Fact]
        public async Task SignUp_ModelUserExistCase()
        {
            // Arrange
            var userId = TestData.UserId;

            var users = TestData.GetUsers().FirstOrDefault();

            var bookDto = new SignUpDTO { Email = "dev@sbsc.com" };

            var resultModel = new ApiResponse<bool>("Email belongs to an existing customer");
            _fac.UserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(users));

            // Act
            var result = await _fac.AuthController.SignUp(bookDto) as OkObjectResult;

            // Assert
            _fac.UserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            Assert.Null(result);
        }
    }
}
