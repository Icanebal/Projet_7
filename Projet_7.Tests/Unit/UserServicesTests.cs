using AutoMapper;
using Moq;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Mappings;
using Projet_7.Web.Services;

namespace Projet_7.Tests
{
    public class UserServicesTests
    {
        private readonly Mock<IUserRepository> _repoMock = new();
        private readonly IMapper _mapper;
        private readonly UserService _service;

        public UserServicesTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            _mapper = config.CreateMapper();
            _service = new UserService(_repoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_UserDtos()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "A", FullName = "A A", Role = "Admin" },
                new User { Id = 2, UserName = "B", FullName = "B B", Role = "User" }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.UserName == "A");
            Assert.Contains(result, u => u.UserName == "B");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_User_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_UserDto_When_User_Found()
        {
            var user = new User { Id = 1, UserName = "Test", FullName = "Test User", Role = "Admin" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.UserName);
            Assert.Equal("Test User", result.FullName);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Returns_Null()
        {
            var dto = new UserDto { UserName = "Test", Password = "pass", FullName = "Test User", Role = "Admin" };
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync((User?)null);

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsFailure);
            Assert.Equal("La création de l'utilisateur n'a pas abouti.", result.Error);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_Valid()
        {
            var dto = new UserDto { UserName = "Test", Password = "pass", FullName = "Test User", Role = "Admin" };
            _repoMock.Setup(r => r.CreateAsync(It.Is<User>(u => u.UserName == "Test")))
                .ReturnsAsync((User u) => { u.Id = 1; return u; });

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("Test", result.Value.UserName);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Failure_When_User_Not_Found()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

            var result = await _service.UpdateAsync(1, new UserDto());

            Assert.True(result.IsFailure);
            Assert.Equal("L'utilisateur spécifié n'existe pas.", result.Error);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Valid()
        {
            var userId = 1;
            var dto = new UserDto { Id = userId, UserName = "Updated", Password = "pass", FullName = "Updated User", Role = "User" };
            var existing = new User { Id = userId, UserName = "Old", FullName = "Old User", Role = "Admin" };
            _repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(userId, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.Id);
            Assert.Equal("Updated", result.Value.UserName);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_User_Deleted()
        {
            var user = new User { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _repoMock.Setup(r => r.DeleteAsync(user)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_User_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

            var result = await _service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
