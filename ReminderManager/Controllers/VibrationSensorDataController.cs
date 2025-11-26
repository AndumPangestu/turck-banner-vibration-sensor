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
    public class VibrationSensorDataController : ControllerBase
    {

        private readonly IVibrationSensorDataService _service;

        public VibrationSensorDataController(IVibrationSensorDataService service)
        {
            _service = service;
        }

        // GET with filter & pagination
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<IEnumerable<VibrationSensorDataResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        [Route("/api/vibration-sensor-data")]
        public async Task<IActionResult> Get([FromQuery] VibrationSensorDataFilterRequest filter)
        {
            var result = await _service.GetVibrationDataAsync(filter);
            return Ok(ResponseHelper.SendSuccess(result.Data, 200, "Get vibration sensor data success", result.Pagination));
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        [Route("/api/vibration-sensor-data/analyze")]
        public async Task<IActionResult> AnalyzeData([FromQuery] VibrationSensorDataFilterRequest filter)
        {
            var result = await _service.AnalyzeData(filter);
            return Ok(ResponseHelper.SendSuccess(result, 200, "Get analyzed vibration sensor data success"));
        }
    }
}
