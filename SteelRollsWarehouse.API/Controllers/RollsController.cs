using Microsoft.AspNetCore.Mvc;
using SteelRollsWarehouse.Application.Services;
using SteelRollsWarehouse.Domain.DTOs;

namespace SteelRollsWarehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RollsController : ControllerBase
    {
        private readonly IRollService _rollService;
        private readonly ILogger<RollsController> _logger;

        public RollsController(IRollService rollService, ILogger<RollsController> logger)
        {
            _rollService = rollService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RollDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RollDto>> AddRoll([FromBody] CreateRollRequest request)
        {
            try
            {
                var roll = await _rollService.AddRollAsync(request);
                return CreatedAtAction(nameof(AddRoll), new { id = roll.Id }, roll);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding roll");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(RollDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RollDto>> DeleteRoll(int id)
        {
            try
            {
                var roll = await _rollService.DeleteRollAsync(id);
                if (roll == null)
                    return NotFound(new { error = $"Roll with id {id} not found" });

                return Ok(roll);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting roll with id {id}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RollDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RollDto>>> GetRolls([FromQuery] RollFilter filter)
        {
            try
            {
                var rolls = await _rollService.GetRollsAsync(filter);
                return Ok(rolls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rolls");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("statistics")]
        [ProducesResponseType(typeof(StatisticsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StatisticsResponse>> GetStatistics([FromQuery] StatisticsRequest request)
        {
            try
            {
                var statistics = await _rollService.GetStatisticsAsync(request);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}