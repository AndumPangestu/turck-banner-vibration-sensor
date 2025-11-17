using Microsoft.AspNetCore.Mvc;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.DTO;
using ReminderManager.Application.Common.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ReminderManager.Api.Controllers
{
    [Route("api/thresholds")]
    [ApiController]
    public class ThresholdController : ControllerBase
    {
        private readonly IThresholdService _service;

        public ThresholdController(IThresholdService service)
        {
            _service = service;
        }

        // GET with filter & pagination
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<IEnumerable<ThresholdResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] ThresholdFilterRequest filter)
        {
            var result = await _service.Get(filter);
            return Ok(ResponseHelper.SendSuccess(result.Data, 200, "Get thresholds success", result.Pagination));
        }

        // GET by ID
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<ThresholdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Show(int id)
        {
            var threshold = await _service.Show(id);
            if (threshold == null)
            {
                return NotFound(ResponseHelper.SendError<string>("Threshold not found", 404));
            }

            return Ok(ResponseHelper.SendSuccess(threshold, 200, "Get threshold by id success"));
        }

        // CREATE
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<ThresholdResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] ThresholdRequest request)
        {
            var threshold = await _service.Create(request);
            return CreatedAtAction(nameof(Show), new { id = threshold.Id },
                ResponseHelper.SendSuccess(threshold, 201, "Threshold created successfully"));
        }

        // UPDATE
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<ThresholdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] ThresholdRequest request)
        {
            var threshold = await _service.Update(id, request);
            if (threshold == null)
            {
                return NotFound(ResponseHelper.SendError<string>("Threshold not found", 404));
            }

            return Ok(ResponseHelper.SendSuccess(threshold, 200, "Threshold updated successfully"));
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseSuccess<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.Delete(id);
            if (!success)
            {
                return NotFound(ResponseHelper.SendError<string>("Threshold not found", 404));
            }

            return Ok(ResponseHelper.SendSuccess("Threshold deleted successfully", 200));
        }
    }
}
