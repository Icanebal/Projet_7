using AutoMapper;
using Moq;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Mappings;
using Projet_7.Web.Services;

namespace Projet_7.Tests.Unit
{
    public class RatingServicesTests
    {
        private readonly Mock<IRatingRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly RatingService _service;

        public RatingServicesTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RatingProfile>());
            _mapper = config.CreateMapper();
            _service = new RatingService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_RatingDtos()
        {
            var ratings = new List<Rating>
            {
                new Rating { Id = 1, MoodysRating = "A", SandPRating = "A", FitchRating = "A" },
                new Rating { Id = 2, MoodysRating = "B", SandPRating = "B", FitchRating = "B" }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ratings);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.MoodysRating == "A" && r.SandPRating == "A");
            Assert.Contains(result, r => r.MoodysRating == "B" && r.SandPRating == "B");
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
            var rating = new Rating { Id = 1, MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rating);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("A", result.MoodysRating);
            Assert.Equal("A", result.SandPRating);
            Assert.Equal("A", result.FitchRating);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new RatingDto { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Rating>())).ReturnsAsync((Rating?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de la note n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new RatingDto { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _repoMock.Setup(r => r.CreateAsync(It.Is<Rating>(r => r.MoodysRating == "A" && r.SandPRating == "A" && r.FitchRating == "A")))
                .ReturnsAsync((Rating r) => { r.Id = 1; return r; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("A", result.Value.MoodysRating);
            Assert.Equal("A", result.Value.SandPRating);
            Assert.Equal("A", result.Value.FitchRating);
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
            var existing = new Rating { Id = ratingId, MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _repoMock.Setup(r => r.GetByIdAsync(ratingId)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(ratingId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(ratingId, result.Value.Id);
            Assert.Equal("B", result.Value.MoodysRating);
            Assert.Equal("B", result.Value.SandPRating);
            Assert.Equal("B", result.Value.FitchRating);
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
