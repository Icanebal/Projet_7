using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Mappings;
using Projet_7.Data.Data;
using Projet_7.Data.Repositories;
using Projet_7.Web.Controllers;
using Projet_7.Web.Services;

namespace Projet_7.Tests.Integration
{
    public class UsersControllerIntegrationTests
    {
        private readonly UsersController _controller;
        private readonly LocalDbContext _context;

        public UsersControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("UsersDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new UserRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
            var mapper = config.CreateMapper();

            var service = new UserService(repository, mapper);
            _controller = new UsersController(service);
        }

        [Fact]
        public async Task GetAllUsers_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
        }

        [Fact]
        public async Task GetUserById_Should_Return_Ok_When_Found()
        {
            var user = new User { UserName = "Test", Password = "pass", FullName = "Test User", Role = "Admin" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.GetUserById(user.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal("Test", dto.UserName);
        }

        [Fact]
        public async Task GetUserById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetUserById(-1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateUser_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new UserDto { UserName = "Created", Password = "pass", FullName = "Created User", Role = "User" };

            var result = await _controller.CreateUser(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returnDto = Assert.IsType<UserDto>(created.Value);
            Assert.Equal("Created", returnDto.UserName);

            var saved = await _context.Users.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateUser_Should_Return_BadRequest_When_InvalidModel()
        {
            var dto = new UserDto { UserName = null, Password = "pass", FullName = "User", Role = "User" };
            _controller.ModelState.AddModelError("UserName", "Le nom d'utilisateur est obligatoire.");

            var result = await _controller.CreateUser(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);
            Assert.True(modelState.ContainsKey("UserName"));
        }

        [Fact]
        public async Task UpdateUser_Should_Return_Ok_With_Updated_Values()
        {
            var user = new User { UserName = "Test", Password = "pass", FullName = "User", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var dto = new UserDto { Id = user.Id, UserName = "Updated", Password = "pass", FullName = "Updated User", Role = "Admin" };

            var result = await _controller.UpdateUser(user.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<UserDto>(ok.Value);
            Assert.Equal("Updated", returned.UserName);
        }

        [Fact]
        public async Task UpdateUser_Should_Return_NotFound_When_NotExists()
        {
            var dto = new UserDto { UserName = "X", Password = "pass", FullName = "User", Role = "User" };

            var result = await _controller.UpdateUser(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Should_Return_NoContent_And_Remove()
        {
            var user = new User { UserName = "Test", Password = "pass", FullName = "User", Role = "User" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var result = await _controller.DeleteUser(user.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Users.FindAsync(user.Id));
        }

        [Fact]
        public async Task DeleteUser_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.DeleteUser(-1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
