//using Microsoft.AspNetCore.Mvc;
//using ReminderManager.Application.Interfaces;
//using ReminderManager.Domain.DTO;
//using ReminderManager.Application.Common.Helpers;
//using Microsoft.AspNetCore.Authorization;

//namespace ReminderManager.Api.Controllers
//{
//    [Route("api/racks")]
//    [ApiController]
//    public class RackController : ControllerBase
//    {
//        private readonly IRackService _service;

//        public RackController(IRackService service)
//        {
//            _service = service;
//        }

//        // GET with filter & pagination
//        [HttpGet]
//        [ProducesResponseType(typeof(ResponseSuccess<Pageable<RackResponse>>), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> Get([FromQuery] RackFilterRequest filter)
//        {
//            var result = await _service.Get(filter);
//            return Ok(ResponseHelper.SendSuccess(result, 200, "Get racks success"));
//        }

//        // GET by ID
//        [HttpGet("{id:int}")]
//        [ProducesResponseType(typeof(ResponseSuccess<RackResponse>), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> Show(int id)
//        {
//            var rack = await _service.Show(id);
//            if (rack == null)
//            {
//                return NotFound(ResponseHelper.SendError<string>("Rack not found", 404));
//            }

//            return Ok(ResponseHelper.SendSuccess(rack, 200, "Get rack by id success"));
//        }

//        // CREATE
//        [HttpPost]
//        [ProducesResponseType(typeof(ResponseSuccess<RackResponse>), StatusCodes.Status201Created)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> Create([FromBody] RackRequest request)
//        {
//            var rack = await _service.Create(request);
//            return CreatedAtAction(nameof(Show), new { id = rack.Id },
//                ResponseHelper.SendSuccess(rack, 201, "Rack created successfully"));
//        }

//        // UPDATE
//        [HttpPut("{id:int}")]
//        [ProducesResponseType(typeof(ResponseSuccess<RackResponse>), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> Update(int id, [FromBody] RackRequest request)
//        {
//            var rack = await _service.Update(id, request);
//            if (rack == null)
//            {
//                return NotFound(ResponseHelper.SendError<string>("Rack not found", 404));
//            }

//            return Ok(ResponseHelper.SendSuccess(rack, 200, "Rack updated successfully"));
//        }

//        // DELETE
//        [HttpDelete("{id:int}")]
//        [ProducesResponseType(typeof(ResponseSuccess<string>), StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status404NotFound)]
//        [ProducesResponseType(typeof(ResponseError<string>), StatusCodes.Status500InternalServerError)]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var success = await _service.Delete(id);
//            if (!success)
//            {
//                return NotFound(ResponseHelper.SendError<string>("Rack not found", 404));
//            }

//            return Ok(ResponseHelper.SendSuccess("Rack deleted successfully", 200));
//        }
//    }
//}
