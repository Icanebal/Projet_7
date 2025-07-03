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

namespace Projet_7.Tests.Integration
{
    public class TradesControllerTests
    {
        private readonly TradesController _controller;
        private readonly LocalDbContext _context;

        public TradesControllerTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("TradesDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new TradeRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TradeProfile>());
            var mapper = config.CreateMapper();

            var service = new TradeService(repository, mapper);
            _controller = new TradesController(service);
        }

        [Fact]
        public async Task GetAllTrades_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllTrades();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<TradeDto>>(ok.Value);
        }

        [Fact]
        public async Task GetTradeById_Should_Return_Ok_When_Found()
        {
            var trade = new Trade { Account = "Test" };
            _context.Trades.Add(trade);
            _context.SaveChanges();

            var result = await _controller.GetTradeById(trade.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TradeDto>(ok.Value);
            Assert.Equal("Test", dto.Account);
        }

        [Fact]
        public async Task GetTradeById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetTradeById(-1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateTrade_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new TradeDto { Account = "Created" };

            var result = await _controller.CreateTrade(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returnDto = Assert.IsType<TradeDto>(created.Value);
            Assert.Equal("Created", returnDto.Account);

            var saved = await _context.Trades.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateTrade_Should_Return_BadRequest_When_InvalidModel()
        {
            var dtoInvalid = new TradeDto { Account = "", BuyQuantity = 10, SellQuantity = 10 };
            _controller.ModelState.AddModelError("Account", "Le champ Account est requis.");

            var resultInvalid = await _controller.CreateTrade(dtoInvalid);

            var badRequest1 = Assert.IsType<BadRequestObjectResult>(resultInvalid);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest1.Value);
            Assert.True(modelState.ContainsKey("Account"));
        }

        [Fact]
        public async Task UpdateTrade_Should_Return_Ok_With_Updated_Values()
        {
            var trade = new Trade { Account = "Test" };
            _context.Trades.Add(trade);
            _context.SaveChanges();

            var dto = new TradeDto { Id = trade.Id, Account = "Updated" };

            var result = await _controller.UpdateTrade(trade.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<TradeDto>(ok.Value);
            Assert.Equal("Updated", returned.Account);
        }

        [Fact]
        public async Task UpdateTrade_Should_Return_NotFound_When_NotExists()
        {
            var dto = new TradeDto { Account = "X" };

            var result = await _controller.UpdateTrade(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTrade_Should_Return_NoContent_And_Remove()
        {
            var trade = new Trade { Account = "Test" };
            _context.Trades.Add(trade);
            _context.SaveChanges();

            var result = await _controller.DeleteTrade(trade.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Trades.FindAsync(trade.Id));
        }

        [Fact]
        public async Task DeleteTrade_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.DeleteTrade(-1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
