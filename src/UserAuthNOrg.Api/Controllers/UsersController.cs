using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthNOrg.Infrastructure.Interfaces;

namespace UserAuthNOrg.Api.Controllers
{
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        private readonly IUsersServices _userService;

        public UsersController(IUsersServices userService)
        {
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _userService.GetUser(id);

            if (result.StatusCode == Utilities.Enums.StatusCode.NotFound)
                return NotFound(result);

            if (result.StatusCode == Utilities.Enums.StatusCode.ERROR)
                return StatusCode(500, result);

            return Ok(result);
        }
    }
}
