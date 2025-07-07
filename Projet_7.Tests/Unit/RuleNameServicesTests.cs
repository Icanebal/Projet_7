using AutoMapper;
using Moq;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Mappings;
using Projet_7.Web.Services;

namespace Projet_7.Tests.Unit
{
    public class RuleNameServicesTests
    {
        private readonly Mock<IRuleNameRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly RuleNameService _service;

        public RuleNameServicesTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RuleNameProfile>());
            _mapper = config.CreateMapper();
            _service = new RuleNameService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_RuleNameDtos()
        {
            var ruleNames = new List<RuleName>
            {
                new RuleName { Id = 1, Name = "A" },
                new RuleName { Id = 2, Name = "B" }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ruleNames);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "A");
            Assert.Contains(result, r => r.Name == "B");
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
            var ruleName = new RuleName { Id = 1, Name = "Test" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ruleName);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new RuleNameDto { Name = "Test" };
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<RuleName>())).ReturnsAsync((RuleName?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de la règle n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new RuleNameDto { Name = "Test" };
            _repoMock.Setup(r => r.CreateAsync(It.Is<RuleName>(r => r.Name == "Test")))
                .ReturnsAsync((RuleName r) => { r.Id = 1; return r; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
            Assert.Equal("Test", result.Value.Name);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_RuleName_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((RuleName?)null);

            var result = await _service.UpdateAsync(1, new RuleNameDto());

            Assert.True(result.IsFailure);
            Assert.Equal("La règle spécifiée n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var ruleNameId = 1;
            var dto = new RuleNameDto { Id = ruleNameId, Name = "Updated" };
            var existing = new RuleName { Id = ruleNameId, Name = "Old" };
            _repoMock.Setup(r => r.GetByIdAsync(ruleNameId)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

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
