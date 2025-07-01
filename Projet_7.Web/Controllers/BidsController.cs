using Microsoft.AspNetCore.Mvc;
using Projet_7.Core.DTO;
using Projet_7.Web.Services;

namespace Projet_7.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly BidService _bidService;

        public BidsController(BidService bidService)
        {
            _bidService = bidService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BidDto>>> GetAllBids()
        {
            var bids = await _bidService.GetAllAsync();
            return Ok(bids);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BidDto>> GetBidById(int id)
        {
            var bid = await _bidService.GetByIdAsync(id);
            if (bid == null)
                return NotFound();

            return Ok(bid);
        }

        [HttpPost]
        public async Task<ActionResult<BidDto>> CreateBid([FromBody] BidDto bidDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bidService.CreateAsync(bidDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetBidById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBid(int id, BidDto bidDto)
        {
            var result = await _bidService.UpdateAsync(id, bidDto);
            if (result.IsFailure)
                return NotFound();

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var success = await _bidService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

