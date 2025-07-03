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
    public class BidsControllerTests
    {
        private readonly BidsController _controller;
        private readonly LocalDbContext _context;

        public BidsControllerTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("BidsDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new BidRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<BidProfile>());
            var mapper = config.CreateMapper();

            var service = new BidService(repository, mapper);
            _controller = new BidsController(service);
        }

        [Fact]
        public async Task GetAllBids_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllBids();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<BidDto>>(ok.Value);
        }

        [Fact]
        public async Task GetBidById_Should_Return_Ok_When_Found()
        {
            var bid = new Bid { Account = "Test", BidType = "Spot", BidQuantity = 10 };
            _context.Bids.Add(bid);
            _context.SaveChanges();

            var result = await _controller.GetBidById(bid.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<BidDto>(ok.Value);
            Assert.Equal("Test", dto.Account);
        }

        [Fact]
        public async Task GetBidById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetBidById(-1);

            Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task CreateBid_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new BidDto { Account = "Created", BidType = "Limit", BidQuantity = 20 };

            var result = await _controller.CreateBid(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnDto = Assert.IsType<BidDto>(created.Value);
            Assert.Equal("Created", returnDto.Account);

            var saved = await _context.Bids.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateBid_Should_Return_BadRequest_When_InvalidModel()
        {
            var dtoInvalid = new BidDto { Account = "", BidType = "X", BidQuantity = 50 };
            _controller.ModelState.AddModelError("Account", "Le compte est obligatoire.");

            var resultInvalid = await _controller.CreateBid(dtoInvalid);

            var badRequest1 = Assert.IsType<BadRequestObjectResult>(resultInvalid.Result);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest1.Value);
            Assert.True(modelState.ContainsKey("Account"));
        }

        [Fact]
        public async Task UpdateBid_Should_Return_Ok_With_Updated_Values()
        {
            var bid = new Bid { Account = "ToUpdate", BidType = "Old", BidQuantity = 5 };
            _context.Bids.Add(bid);
            _context.SaveChanges();

            var updatedDto = new BidDto { Id = bid.Id, Account = "Updated", BidType = "New", BidQuantity = 15 };

            var result = await _controller.UpdateBid(bid.Id, updatedDto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<BidDto>(ok.Value);
            Assert.Equal("Updated", dto.Account);
        }

        [Fact]
        public async Task UpdateBid_Should_Return_NotFound_When_Bid_NotExists()
        {
            var dto = new BidDto { Account = "X", BidType = "Y", BidQuantity = 10 };

            var result = await _controller.UpdateBid(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task DeleteBid_Should_Return_NoContent_And_Remove()
        {
            var bid = new Bid { Account = "ToDelete", BidType = "Spot", BidQuantity = 30 };
            _context.Bids.Add(bid);
            _context.SaveChanges();

            var result = await _controller.DeleteBid(bid.Id);

            Assert.IsType<NoContentResult>(result);

            var deleted = await _context.Bids.FindAsync(bid.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteBid_Should_Return_NotFound_When_Bid_NotExists()
        {
            var result = await _controller.DeleteBid(-1);

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
