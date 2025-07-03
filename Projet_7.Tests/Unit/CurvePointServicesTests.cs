using AutoMapper;
using Moq;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Mappings;
using Projet_7.Web.Services;

namespace Projet_7.Tests.Unit
{
    public class CurvePointServiceTests
    {
        private readonly Mock<ICurvePointRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly CurvePointService _service;

        public CurvePointServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CurvePointProfile>());
            _mapper = config.CreateMapper();
            _service = new CurvePointService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_Dtos()
        {
            var data = new List<CurvePoint>
            {
                new CurvePoint { Id = 1, CurveId = 1, Term = 2.0, CurvePointValue = 1.5 },
                new CurvePoint { Id = 2, CurveId = 2, Term = 3.0, CurvePointValue = 2.5 }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.CurveId == 1 && c.Term == 2.0);
            Assert.Contains(result, c => c.CurveId == 2 && c.Term == 3.0);
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
            var entity = new CurvePoint { Id = 1, CurveId = 2, Term = 3.5, CurvePointValue = 1.1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal((byte)2, result.CurveId);
            Assert.Equal(3.5, result.Term);
            Assert.Equal(1.1, result.CurvePointValue);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repo_Returns_Null()
        {
            var dto = new CurvePointDto { CurveId = 1, Term = 5, CurvePointValue = 0.5 };
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<CurvePoint>())).ReturnsAsync((CurvePoint?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création du point n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new CurvePointDto { CurveId = 1, Term = 5, CurvePointValue = 0.5 };
            _repoMock.Setup(r => r.CreateAsync(It.Is<CurvePoint>(c => c.CurveId == 1 && c.Term == 5 && c.CurvePointValue == 0.5)))
                .ReturnsAsync((CurvePoint c) => { c.Id = 1; return c; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
            Assert.Equal((byte)1, result.Value.CurveId);
            Assert.Equal(5, result.Value.Term);
            Assert.Equal(0.5, result.Value.CurvePointValue);
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
            var existing = new CurvePoint { Id = id, CurveId = 1, Term = 1.0, CurvePointValue = 0.5 };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(id, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(id, result.Value!.Id);
            Assert.Equal((byte)2, result.Value.CurveId);
            Assert.Equal(3.0, result.Value.Term);
            Assert.Equal(1.25, result.Value.CurvePointValue);
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
