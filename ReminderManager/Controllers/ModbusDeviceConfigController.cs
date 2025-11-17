using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReminderManager.Application.Common.Helpers;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModbusDeviceConfigController : ControllerBase
    {

        private readonly IModbusDeviceConfigService _service;

        public ModbusDeviceConfigController(IModbusDeviceConfigService service)
        {
            _service = service;
        }

        // GET with filter & pagination
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<IEnumerable<ModbusDeviceConfigResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        [Route("/api/modbus-devices")]
        public async Task<IActionResult> Get([FromQuery] ModbusDeviceConfigFilterRequest filter)
        {
            var result = await _service.Get(filter);
            return Ok(ResponseHelper.SendSuccess(result.Data, 200, "Get modbus device success", result.Pagination));
        }
    }
}
