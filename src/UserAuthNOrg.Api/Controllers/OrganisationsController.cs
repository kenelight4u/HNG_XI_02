using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthNOrg.Core.ViewModel;
using UserAuthNOrg.Infrastructure.Interfaces;
using UserAuthNOrg.Utilities.Extensions;

namespace UserAuthNOrg.Api.Controllers
{
    [ApiVersion("1.0")]
    public class OrganisationsController : BaseController
    {
        private readonly IOrganizationServices _orgService;

        public OrganisationsController(IOrganizationServices orgService)
        {
            _orgService = orgService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var result = await _orgService.GetOrganizations();

            if (result.StatusCode == Utilities.Enums.StatusCode.ERROR)
                return StatusCode(500, result);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetAllOrganization(Guid orgId)
        {
            var result = await _orgService.GetOrganization(orgId);

            if (result.StatusCode == Utilities.Enums.StatusCode.NotFound)
                return NotFound(result);

            if (result.StatusCode == Utilities.Enums.StatusCode.ERROR)
                return StatusCode(500, result);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost()]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDTO model)
        {
            var (status, errors) = ValidatePayload(model);

            if (status)
            {
                var result = await _orgService.CreateOrganization(model);

                if (result.StatusCode == Utilities.Enums.StatusCode.ERROR)
                    return StatusCode(500, result);

                return StatusCode(201, result);
            }

            var response = new ApiResponse<List<Error>>(errors);
            return StatusCode(422, response);
        }

        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganization(Guid orgId, [FromBody] ViewUserDto model)
        {
            var (status, errors) = ValidatePayload(model);

            if (status)
            {
                var result = await _orgService.UpdateUsersOrganizations(orgId, model);

                if (result.StatusCode == Utilities.Enums.StatusCode.NotFound)
                    return StatusCode(404, result);

                if (result.StatusCode == Utilities.Enums.StatusCode.BadRequest)
                    return BadRequest(result);

                if (result.StatusCode == Utilities.Enums.StatusCode.ERROR)
                    return StatusCode(500, result);

                return Ok(result);
            }

            var response = new ApiResponse<List<Error>>(errors);
            return StatusCode(422, response);
        }

        private static (bool status, List<Error> errors) ValidatePayload(CreateOrganizationDTO model)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(model.Name))
                errors.Add(new() { Field = "Name", Message = "Organization Name cannot be null!" });

            if (!errors.Any())
                return (true, errors);

            return (false, errors);
        }

        private static (bool status, List<Error> errors) ValidatePayload(ViewUserDto model)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(model.Userid))
                errors.Add(new() { Field = "userId", Message = "User Id cannot be null!" });

            if (!errors.Any())
                return (true, errors);

            return (false, errors);
        }

    }
}
