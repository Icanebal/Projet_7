using Moq;
using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class TradeServicesTests
    {
        private readonly Mock<ITradeRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly TradeService _service;

        public TradeServicesTests()
        {
            _service = new TradeService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_TradeDtos()
        {
            var trades = new List<Trade> { new Trade(), new Trade() };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(trades);
            _mapperMock.Setup(m => m.Map<IEnumerable<TradeDto>>(trades)).Returns(new List<TradeDto>());

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Trade_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trade?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_TradeDto_When_Trade_Found()
        {
            var trade = new Trade { Id = 1 };
            var dto = new TradeDto { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trade);
            _mapperMock.Setup(m => m.Map<TradeDto>(trade)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new TradeDto { Account = "Test" };
            var trade = new Trade { Id = 0 };

            _mapperMock.Setup(m => m.Map<Trade>(dto)).Returns(trade);
            _repoMock.Setup(r => r.CreateAsync(trade)).ReturnsAsync((Trade?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création du trade n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new TradeDto { Account = "Test" };
            var trade = new Trade { Id = 1 };
            var returnedDto = new TradeDto { Id = 1 };

            _mapperMock.Setup(m => m.Map<Trade>(dto)).Returns(trade);
            _repoMock.Setup(r => r.CreateAsync(trade)).ReturnsAsync(trade);
            _mapperMock.Setup(m => m.Map<TradeDto>(trade)).Returns(returnedDto);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_Trade_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trade?)null);

            var result = await _service.UpdateAsync(1, new TradeDto());

            Assert.True(result.IsFailure);
            Assert.Equal("Le trade spécifié n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var tradeId = 1;
            var dto = new TradeDto { Id = tradeId, Account = "Updated" };
            var existing = new Trade { Id = tradeId };
            var resultDto = new TradeDto { Id = tradeId, Account = "Updated" };

            _repoMock.Setup(r => r.GetByIdAsync(tradeId)).ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(dto, existing))
                .Callback<TradeDto, Trade>((src, dest) =>
                {
                    dest.Account = src.Account;
                    dest.BuyQuantity = src.BuyQuantity;
                    dest.SellQuantity = src.SellQuantity;
                    dest.DealType = src.DealType;
                });

            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<TradeDto>(existing)).Returns(resultDto);

            var result = await _service.UpdateAsync(tradeId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(tradeId, result.Value!.Id);
            Assert.Equal("Updated", result.Value.Account);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Trade_Deleted()
        {
            var trade = new Trade { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trade);
            _repoMock.Setup(r => r.DeleteAsync(trade)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Trade_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trade?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
