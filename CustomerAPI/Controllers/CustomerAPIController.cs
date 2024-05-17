using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        [HttpGet("defaultTimeout")]
        public async Task<IActionResult> GetCustomerWithDefaultTimeoutAsync()
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpGet("twosecondtimeout/{waitSeconds:int}")]
        [RequestTimeout(2000)]
        public async Task<IActionResult> GetCustomerWithTwoSecondTimeoutAsync([FromRoute] int waitSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("threesecondtimeout/{waitSeconds:int}")]
        [RequestTimeout("threesecondpolicy")]
        public async Task<IActionResult> GetCustomerWithThreeSecondTimeoutPolicyAsync([FromRoute] int waitSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("customstatuscode/{waitSeconds:int}")]
        [RequestTimeout("customstatuscode")]
        public async Task<IActionResult> GetCustomerWithCustomStatusCodePolicyAsync([FromRoute] int waitSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("customdelegatepolicy/{waitSeconds:int}")]
        [RequestTimeout("customdelegatepolicy")]
        public async Task<IActionResult> GetCustomerWithCustomDelegateAsync([FromRoute] int waitSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            return Ok();
        }

        [HttpGet("disableTimeout/{waitSeconds:int}")]
        [DisableRequestTimeout]
        public async Task<IActionResult> GetCustomerWithNoTimeoutAsync([FromRoute] int waitSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            return Ok();
        }
    }
}
