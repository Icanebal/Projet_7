using Moq;
using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class RatingServicesTests
    {
        private readonly Mock<IRatingRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly RatingService _service;

        public RatingServicesTests()
        {
            _service = new RatingService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_RatingDtos()
        {
            var ratings = new List<Rating> { new Rating(), new Rating() };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ratings);
            _mapperMock.Setup(m => m.Map<IEnumerable<RatingDto>>(ratings)).Returns(new List<RatingDto>());

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Rating_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Rating?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_RatingDto_When_Rating_Found()
        {
            var rating = new Rating { Id = 1 };
            var dto = new RatingDto { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rating);
            _mapperMock.Setup(m => m.Map<RatingDto>(rating)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new RatingDto { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            var rating = new Rating { Id = 0 };

            _mapperMock.Setup(m => m.Map<Rating>(dto)).Returns(rating);
            _repoMock.Setup(r => r.CreateAsync(rating)).ReturnsAsync((Rating?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de la note n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new RatingDto { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            var rating = new Rating { Id = 1 };
            var returnedDto = new RatingDto { Id = 1 };

            _mapperMock.Setup(m => m.Map<Rating>(dto)).Returns(rating);
            _repoMock.Setup(r => r.CreateAsync(rating)).ReturnsAsync(rating);
            _mapperMock.Setup(m => m.Map<RatingDto>(rating)).Returns(returnedDto);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_Rating_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Rating?)null);

            var result = await _service.UpdateAsync(1, new RatingDto());

            Assert.True(result.IsFailure);
            Assert.Equal("La note spécifiée n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var ratingId = 1;
            var dto = new RatingDto { Id = ratingId, MoodysRating = "B", SandPRating = "B", FitchRating = "B" };
            var existing = new Rating { Id = ratingId };
            var resultDto = new RatingDto { Id = ratingId, MoodysRating = "B", SandPRating = "B", FitchRating = "B" };

            _repoMock.Setup(r => r.GetByIdAsync(ratingId)).ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(dto, existing))
                .Callback<RatingDto, Rating>((src, dest) =>
                {
                    dest.MoodysRating = src.MoodysRating;
                    dest.SandPRating = src.SandPRating;
                    dest.FitchRating = src.FitchRating;
                    dest.OrderNumber = src.OrderNumber;
                });

            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<RatingDto>(existing)).Returns(resultDto);

            var result = await _service.UpdateAsync(ratingId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(ratingId, result.Value.Id);
            Assert.Equal("B", result.Value.MoodysRating);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Rating_Deleted()
        {
            var rating = new Rating { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rating);
            _repoMock.Setup(r => r.DeleteAsync(rating)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Rating_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Rating?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
