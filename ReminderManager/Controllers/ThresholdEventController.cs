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
    public class ThresholdEventController : ControllerBase
    {

        private readonly IThresholdEventService _service;

        public ThresholdEventController(IThresholdEventService service)
        {
            _service = service;
        }

        // GET with filter & pagination
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<IEnumerable<ThresholdEventResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        [Route("/api/threshold-events")]
        public async Task<IActionResult> Get([FromQuery] ThresholdEventFilterRequest filter)
        {
            var result = await _service.Get(filter);
            return Ok(ResponseHelper.SendSuccess(result.Data, 200, "Get threshold event data success", result.Pagination));
        }


    }
}
