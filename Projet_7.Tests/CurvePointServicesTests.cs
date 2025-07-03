using Moq;
using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class CurvePointServiceTests
    {
        private readonly Mock<ICurvePointRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly CurvePointService _service;

        public CurvePointServiceTests()
        {
            _service = new CurvePointService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_Dtos()
        {
            var data = new List<CurvePoint> { new CurvePoint(), new CurvePoint() };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);
            _mapperMock.Setup(m => m.Map<IEnumerable<CurvePointDto>>(data)).Returns(new List<CurvePointDto>());

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CurvePoint?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Dto_When_Found()
        {
            var entity = new CurvePoint { Id = 1 };
            var dto = new CurvePointDto { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<CurvePointDto>(entity)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repo_Returns_Null()
        {
            var dto = new CurvePointDto { CurveId = 1, Term = 5, CurvePointValue = 0.5 };
            var entity = new CurvePoint();

            _mapperMock.Setup(m => m.Map<CurvePoint>(dto)).Returns(entity);
            _repoMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync((CurvePoint?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création du point n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new CurvePointDto { CurveId = 1, Term = 5, CurvePointValue = 0.5 };
            var entity = new CurvePoint { Id = 1 };
            var returnedDto = new CurvePointDto { Id = 1 };

            _mapperMock.Setup(m => m.Map<CurvePoint>(dto)).Returns(entity);
            _repoMock.Setup(r => r.CreateAsync(entity)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<CurvePointDto>(entity)).Returns(returnedDto);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CurvePoint?)null);

            var result = await _service.UpdateAsync(1, new CurvePointDto());

            Assert.True(result.IsFailure);
            Assert.Equal("Le point spécifié n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var id = 1;
            var dto = new CurvePointDto { Id = id, CurveId = 2, Term = 3.0, CurvePointValue = 1.25 };
            var existing = new CurvePoint { Id = id };
            var updatedDto = new CurvePointDto { Id = id, CurveId = 2, Term = 3.0, CurvePointValue = 1.25 };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(dto, existing))
                .Callback<CurvePointDto, CurvePoint>((src, dest) =>
                {
                    dest.CurveId = src.CurveId;
                    dest.Term = src.Term;
                    dest.CurvePointValue = src.CurvePointValue;
                });

            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CurvePointDto>(existing)).Returns(updatedDto);

            var result = await _service.UpdateAsync(id, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(id, result.Value!.Id);
            Assert.Equal((byte)2, result.Value.CurveId);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            var point = new CurvePoint { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(point);
            _repoMock.Setup(r => r.DeleteAsync(point)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CurvePoint?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
