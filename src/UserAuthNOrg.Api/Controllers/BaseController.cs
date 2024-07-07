using Microsoft.AspNetCore.Mvc;

namespace UserAuthNOrg.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        //public IActionResult ApiResponse<T>(T data = default, string message = "",
        //    StatusCode codes = Utilities.Enums.StatusCode.OK, params Error[] errors)
        //{
        //    var response = new ApiResponse<T>(data, message, codes, errors);
        //    response.Message = message ?? response.StatusCode.GetDescription();
        //    return Ok(response);
        //}
        
    }
}
