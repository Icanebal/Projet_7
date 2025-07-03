using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Mappings;
using Projet_7.Data.Data;
using Projet_7.Data.Repositories;
using Projet_7.Web.Controllers;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class RuleNamesControllerIntegrationTests
    {
        private readonly RuleNamesController _controller;
        private readonly LocalDbContext _context;

        public RuleNamesControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("RuleNamesDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new RuleNameRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<RuleNameProfile>());
            var mapper = config.CreateMapper();

            var service = new RuleNameService(repository, mapper);
            _controller = new RuleNamesController(service);
        }

        [Fact]
        public async Task GetAllRuleNames_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllRuleNames();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<RuleNameDto>>(ok.Value);
        }

        [Fact]
        public async Task GetRuleNameById_Should_Return_Ok_When_Found()
        {
            var ruleName = new RuleName { Name = "Test" };
            _context.RuleNames.Add(ruleName);
            _context.SaveChanges();

            var result = await _controller.GetRuleNameById(ruleName.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RuleNameDto>(ok.Value);
            Assert.Equal(ruleName.Name, dto.Name);
        }

        [Fact]
        public async Task GetRuleNameById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetRuleNameById(-1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateRuleName_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new RuleNameDto { Name = "Test" };

            var result = await _controller.CreateRuleName(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returnDto = Assert.IsType<RuleNameDto>(created.Value);
            Assert.Equal("Test", returnDto.Name);

            var saved = await _context.RuleNames.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateRuleName_Should_Return_BadRequest_When_InvalidModel()
        {
            var dto = new RuleNameDto { Name = null };
            _controller.ModelState.AddModelError("Name", "Le champ Name est requis.");

            var result = await _controller.CreateRuleName(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);
            Assert.True(modelState.ContainsKey("Name"));
        }

        [Fact]
        public async Task UpdateRuleName_Should_Return_Ok_With_Updated_Values()
        {
            var ruleName = new RuleName { Name = "Test" };
            _context.RuleNames.Add(ruleName);
            _context.SaveChanges();

            var dto = new RuleNameDto { Id = ruleName.Id, Name = "Updated" };

            var result = await _controller.UpdateRuleName(ruleName.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<RuleNameDto>(ok.Value);
            Assert.Equal("Updated", returned.Name);
        }

        [Fact]
        public async Task UpdateRuleName_Should_Return_NotFound_When_NotExists()
        {
            var dto = new RuleNameDto { Name = "X" };

            var result = await _controller.UpdateRuleName(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteRuleName_Should_Return_NoContent_And_Remove()
        {
            var ruleName = new RuleName { Name = "Test" };
            _context.RuleNames.Add(ruleName);
            _context.SaveChanges();

            var result = await _controller.DeleteRuleName(ruleName.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.RuleNames.FindAsync(ruleName.Id));
        }

        [Fact]
        public async Task DeleteRuleName_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.DeleteRuleName(-1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
