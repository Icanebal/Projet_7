using Moq;
using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class BidServiceTests
    {
        private readonly Mock<IBidRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly BidService _service;

        public BidServiceTests()
        {
            _service = new BidService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_BidDtos()
        {
            var bids = new List<Bid> { new Bid(), new Bid() };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bids);
            _mapperMock.Setup(m => m.Map<IEnumerable<BidDto>>(bids)).Returns(new List<BidDto>());

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Bid_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Bid)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_BidDto_When_Bid_Found()
        {
            var bid = new Bid { Id = 1 };
            var dto = new BidDto { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bid);
            _mapperMock.Setup(m => m.Map<BidDto>(bid)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new BidDto { Account = "Test", BidType = "Type" };
            var bid = new Bid { Id = 0 };

            _mapperMock.Setup(m => m.Map<Bid>(dto)).Returns(bid);
            _repoMock.Setup(r => r.CreateAsync(bid)).ReturnsAsync((Bid)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de l'offre n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new BidDto { Account = "Test", BidType = "Type" };
            var bid = new Bid { Id = 1 };
            var returnedDto = new BidDto { Id = 1 };

            _mapperMock.Setup(m => m.Map<Bid>(dto)).Returns(bid);
            _repoMock.Setup(r => r.CreateAsync(bid)).ReturnsAsync(bid);
            _mapperMock.Setup(m => m.Map<BidDto>(bid)).Returns(returnedDto);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, actual: result.Value.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_Bid_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Bid)null);

            var result = await _service.UpdateAsync(1, new BidDto());

            Assert.True(result.IsFailure);
            Assert.Equal("L'offre spécifiée n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var bidId = 1;
            var dto = new BidDto { Id = bidId, Account = "Updated", BidType = "New", BidQuantity = 42 };

            var existing = new Bid { Id = bidId, Account = "Old", BidType = "Old", BidQuantity = 10 };
            var resultDto = new BidDto { Id = bidId, Account = "Updated", BidType = "New", BidQuantity = 42 };

            _repoMock.Setup(r => r.GetByIdAsync(bidId)).ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(dto, existing))
                .Callback<BidDto, Bid>((src, dest) =>
                {
                    dest.Account = src.Account;
                    dest.BidType = src.BidType;
                    dest.BidQuantity = src.BidQuantity;
                });

            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<BidDto>(existing))
                .Returns(resultDto);

            var result = await _service.UpdateAsync(bidId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(bidId, result.Value.Id);
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
                     .ReturnsAsync((Bid)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }

    }
}

