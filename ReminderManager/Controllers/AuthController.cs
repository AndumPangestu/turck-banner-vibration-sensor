using Microsoft.AspNetCore.Mvc;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.DTO;
using ReminderManager.Application.Common.Helpers;

namespace ReminderManager.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseSuccess<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            var result = await _service.Login(request);
            return Ok(ResponseHelper.SendSuccess(result, 200, "Login success"));
        }
    }
}
