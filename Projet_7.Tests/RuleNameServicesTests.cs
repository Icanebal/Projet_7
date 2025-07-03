using Moq;
using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class RuleNameServicesTests
    {
        private readonly Mock<IRuleNameRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly RuleNameService _service;

        public RuleNameServicesTests()
        {
            _service = new RuleNameService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_RuleNameDtos()
        {
            var ruleNames = new List<RuleName> { new RuleName(), new RuleName() };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ruleNames);
            _mapperMock.Setup(m => m.Map<IEnumerable<RuleNameDto>>(ruleNames)).Returns(new List<RuleNameDto>());

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_RuleName_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((RuleName?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_RuleNameDto_When_RuleName_Found()
        {
            var ruleName = new RuleName { Id = 1 };
            var dto = new RuleNameDto { Id = 1 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ruleName);
            _mapperMock.Setup(m => m.Map<RuleNameDto>(ruleName)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new RuleNameDto { Name = "Test" };
            var ruleName = new RuleName { Id = 0 };

            _mapperMock.Setup(m => m.Map<RuleName>(dto)).Returns(ruleName);
            _repoMock.Setup(r => r.CreateAsync(ruleName)).ReturnsAsync((RuleName?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création du RuleName n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new RuleNameDto { Name = "Test" };
            var ruleName = new RuleName { Id = 1 };
            var returnedDto = new RuleNameDto { Id = 1 };

            _mapperMock.Setup(m => m.Map<RuleName>(dto)).Returns(ruleName);
            _repoMock.Setup(r => r.CreateAsync(ruleName)).ReturnsAsync(ruleName);
            _mapperMock.Setup(m => m.Map<RuleNameDto>(ruleName)).Returns(returnedDto);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_RuleName_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((RuleName?)null);

            var result = await _service.UpdateAsync(1, new RuleNameDto());

            Assert.True(result.IsFailure);
            Assert.Equal("Le RuleName spécifié n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var ruleNameId = 1;
            var dto = new RuleNameDto { Id = ruleNameId, Name = "Updated" };
            var existing = new RuleName { Id = ruleNameId };
            var resultDto = new RuleNameDto { Id = ruleNameId, Name = "Updated" };

            _repoMock.Setup(r => r.GetByIdAsync(ruleNameId)).ReturnsAsync(existing);

            _mapperMock
                .Setup(m => m.Map(dto, existing))
                .Callback<RuleNameDto, RuleName>((src, dest) =>
                {
                    dest.Name = src.Name;
                    dest.Description = src.Description;
                    dest.Json = src.Json;
                    dest.Template = src.Template;
                    dest.SqlStr = src.SqlStr;
                    dest.SqlPart = src.SqlPart;
                });

            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<RuleNameDto>(existing)).Returns(resultDto);

            var result = await _service.UpdateAsync(ruleNameId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(ruleNameId, result.Value!.Id);
            Assert.Equal("Updated", result.Value.Name);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_RuleName_Deleted()
        {
            var ruleName = new RuleName { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ruleName);
            _repoMock.Setup(r => r.DeleteAsync(ruleName)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_RuleName_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((RuleName?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
