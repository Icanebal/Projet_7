using Microsoft.AspNetCore.Mvc;
using Projet_7.Core.DTO;
using Projet_7.Web.Services;

namespace Projet_7.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _ratingService.GetAllAsync();
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            var rating = await _ratingService.GetByIdAsync(id);
            if (rating == null)
                return NotFound();
            return Ok(rating);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] RatingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _ratingService.CreateAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetRatingById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _ratingService.UpdateAsync(id, dto);
            if (result.IsFailure)
                return NotFound();
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var deleted = await _ratingService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}