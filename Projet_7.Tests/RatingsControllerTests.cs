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

namespace Projet_7.Tests
{
    public class RatingControllerIntegrationTests
    {
        private readonly RatingsController _controller;
        private readonly LocalDbContext _context;

        public RatingControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("RatingsDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new RatingRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<RatingProfile>());
            var mapper = config.CreateMapper();

            var service = new RatingService(repository, mapper);
            _controller = new RatingsController(service);
        }

        [Fact]
        public async Task GetAllRatings_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllRatings();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<RatingDto>>(ok.Value);
        }

        [Fact]
        public async Task GetRatingById_Should_Return_Ok_When_Found()
        {
            var rating = new Rating { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _context.Ratings.Add(rating);
            _context.SaveChanges();

            var result = await _controller.GetRatingById(rating.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<RatingDto>(ok.Value);
            Assert.Equal(rating.MoodysRating, dto.MoodysRating);
        }

        [Fact]
        public async Task GetRatingById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetRatingById(-1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateRating_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new RatingDto { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };

            var result = await _controller.CreateRating(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returnDto = Assert.IsType<RatingDto>(created.Value);
            Assert.Equal("A", returnDto.MoodysRating);

            var saved = await _context.Ratings.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateRating_Should_Return_BadRequest_When_InvalidModel()
        {
            var dto = new RatingDto { MoodysRating = null, SandPRating = "A", FitchRating = "A" };
            _controller.ModelState.AddModelError("MoodysRating", "Le champ MoodysRating est requis.");

            var result = await _controller.CreateRating(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);
            Assert.True(modelState.ContainsKey("MoodysRating"));
        }

        [Fact]
        public async Task UpdateRating_Should_Return_Ok_With_Updated_Values()
        {
            var rating = new Rating { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _context.Ratings.Add(rating);
            _context.SaveChanges();

            var dto = new RatingDto { Id = rating.Id, MoodysRating = "B", SandPRating = "B", FitchRating = "B" };

            var result = await _controller.UpdateRating(rating.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<RatingDto>(ok.Value);
            Assert.Equal("B", returned.MoodysRating);
        }

        [Fact]
        public async Task UpdateRating_Should_Return_NotFound_When_NotExists()
        {
            var dto = new RatingDto { MoodysRating = "X", SandPRating = "X", FitchRating = "X" };

            var result = await _controller.UpdateRating(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteRating_Should_Return_NoContent_And_Remove()
        {
            var rating = new Rating { MoodysRating = "A", SandPRating = "A", FitchRating = "A" };
            _context.Ratings.Add(rating);
            _context.SaveChanges();

            var result = await _controller.DeleteRating(rating.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Ratings.FindAsync(rating.Id));
        }

        [Fact]
        public async Task DeleteRating_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.DeleteRating(-1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
