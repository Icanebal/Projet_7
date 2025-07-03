using Microsoft.AspNetCore.Mvc;
using Projet_7.Core.DTO;
using Projet_7.Web.Services;

namespace Projet_7.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurvePointsController : ControllerBase
    {
        private readonly CurvePointService _curvePointService;

        public CurvePointsController(CurvePointService curvePointService)
        {
            _curvePointService = curvePointService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurvePointDto>>> GetAllCurvePoints()
        {
            var points = await _curvePointService.GetAllAsync();
            return Ok(points);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CurvePointDto>> GetCurvePointById(int id)
        {
            var point = await _curvePointService.GetByIdAsync(id);
            if (point == null)
                return NotFound();

            return Ok(point);
        }

        [HttpPost]
        public async Task<ActionResult<CurvePointDto>> CreateCurvePoint([FromBody] CurvePointDto curvePointDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _curvePointService.CreateAsync(curvePointDto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCurvePointById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurvePoint(int id, CurvePointDto curvePointDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _curvePointService.UpdateAsync(id, curvePointDto);
            if (result.IsFailure)
                return NotFound();

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            var success = await _curvePointService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
