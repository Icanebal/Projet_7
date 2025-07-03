using Microsoft.AspNetCore.Mvc;
using Projet_7.Core.DTO;
using Projet_7.Web.Services;

namespace Projet_7.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly TradeService _service;

        public TradesController(TradeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            var trades = await _service.GetAllAsync();
            return Ok(trades);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(int id)
        {
            var trade = await _service.GetByIdAsync(id);
            if (trade == null)
                return NotFound();
            return Ok(trade);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrade([FromBody] TradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetTradeById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] TradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsFailure)
                return NotFound();
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
