using AutoMapper;
using Moq;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Mappings;
using Projet_7.Web.Services;

namespace Projet_7.Tests.Unit
{
    public class BidServicesTests
    {
        private readonly Mock<IBidRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly BidService _service;

        public BidServicesTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<BidProfile>());
            _mapper = config.CreateMapper();
            _service = new BidService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_BidDtos()
        {
            var bids = new List<Bid>
            {
                new Bid { Id = 1, Account = "A", BidType = "Type1" },
                new Bid { Id = 2, Account = "B", BidType = "Type2" }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bids);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Account == "A" && b.BidType == "Type1");
            Assert.Contains(result, b => b.Account == "B" && b.BidType == "Type2");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Bid_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Bid?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_BidDto_When_Bid_Found()
        {
            var bid = new Bid
            {
                Id = 1,
                Account = "TestAccount",
                BidType = "TestType",
                BidQuantity = 100
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bid);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("TestAccount", result.Account);
            Assert.Equal("TestType", result.BidType);
            Assert.Equal(100, result.BidQuantity);
        }


        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new BidDto
            {
                Account = "Test",
                BidType = "Type",
                BidQuantity = 50
            };

            _repoMock
                .Setup(r => r.CreateAsync(It.IsAny<Bid>()))
                .ReturnsAsync((Bid?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de l'offre n'a pas abouti.", result.Error);
        }


        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new BidDto
            {
                Account = "Test",
                BidType = "Type",
                BidQuantity = 42
            };

            _repoMock
                .Setup(r => r.CreateAsync(It.Is<Bid>(b =>
                    b.Account == "Test" &&
                    b.BidType == "Type" &&
                    b.BidQuantity == 42)))
                .ReturnsAsync((Bid b) =>
                {
                    b.Id = 1;
                    return b;
                });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
            Assert.Equal("Test", result.Value.Account);
            Assert.Equal("Type", result.Value.BidType);
            Assert.Equal(42, result.Value.BidQuantity);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_Bid_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Bid?)null);

            var result = await _service.UpdateAsync(1, new BidDto());

            Assert.True(result.IsFailure);
            Assert.Equal("L'offre spécifiée n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var bidId = 1;

            var dto = new BidDto
            {
                Id = bidId,
                Account = "Updated",
                BidType = "New",
                BidQuantity = 42
            };

            var existing = new Bid
            {
                Id = bidId,
                Account = "Old",
                BidType = "Old",
                BidQuantity = 10
            };

            _repoMock.Setup(r => r.GetByIdAsync(bidId)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(bidId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(bidId, result.Value!.Id);
            Assert.Equal("Updated", result.Value.Account);
            Assert.Equal("New", result.Value.BidType);
            Assert.Equal(42, result.Value.BidQuantity);
        }



        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Bid_Deleted()
        {
            var bid = new Bid { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bid);
            _repoMock.Setup(r => r.DeleteAsync(bid)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Bid_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((Bid?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }

    }
}

