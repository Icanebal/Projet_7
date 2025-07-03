using Microsoft.AspNetCore.Mvc;
using Projet_7.Core.DTO;
using Projet_7.Web.Services;

namespace Projet_7.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RuleNamesController : ControllerBase
    {
        private readonly RuleNameService _service;

        public RuleNamesController(RuleNameService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRuleNames()
        {
            var ruleNames = await _service.GetAllAsync();
            return Ok(ruleNames);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleNameById(int id)
        {
            var ruleName = await _service.GetByIdAsync(id);
            if (ruleName == null)
                return NotFound();
            return Ok(ruleName);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRuleName([FromBody] RuleNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            if (result.IsFailure)
                return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetRuleNameById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsFailure)
                return NotFound();
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
