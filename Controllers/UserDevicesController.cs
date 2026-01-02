using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tallypath.Data;
using Tallypath.Services;

namespace Tallypath.Controllers
{
    [ApiController]
    [Route("api/user-devices")]
    [Authorize]
    public class UserDevicesController : ControllerBase
    {
        private readonly IUserDeviceService _service;

        public UserDevicesController(IUserDeviceService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            var userId = User.GetUserId();

            await _service.RegisterOrUpdateAsync(userId, request);

            return Ok();
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateDevice([FromBody] DeactivateDeviceRequest request)
        {
            var userId = User.GetUserId();

            await _service.DeactivateAsync(userId, request);

            return Ok();
        }
    }

}
