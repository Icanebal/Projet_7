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
    public class CurvePointsControllerIntegrationTests
    {
        private readonly CurvePointsController _controller;
        private readonly LocalDbContext _context;

        public CurvePointsControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase("CurvePointsDbTest")
                .Options;

            _context = new LocalDbContext(options);
            var repository = new CurvePointRepository(_context);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<CurvePointProfile>());
            var mapper = config.CreateMapper();

            var service = new CurvePointService(repository, mapper);
            _controller = new CurvePointsController(service);
        }

        [Fact]
        public async Task GetAllCurvePoints_Should_Return_Ok_With_List()
        {
            var result = await _controller.GetAllCurvePoints();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<CurvePointDto>>(ok.Value);
        }

        [Fact]
        public async Task GetCurvePointById_Should_Return_Ok_When_Found()
        {
            var point = new CurvePoint { CurveId = 1, Term = 2.5, CurvePointValue = 1.2 };
            _context.CurvePoints.Add(point);
            _context.SaveChanges();

            var result = await _controller.GetCurvePointById(point.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<CurvePointDto>(ok.Value);
            Assert.Equal(point.CurveId, dto.CurveId);
        }

        [Fact]
        public async Task GetCurvePointById_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.GetCurvePointById(-1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateCurvePoint_Should_Return_CreatedAtAction_And_Save()
        {
            var dto = new CurvePointDto { CurveId = 1, Term = 5, CurvePointValue = 3.5 };

            var result = await _controller.CreateCurvePoint(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnDto = Assert.IsType<CurvePointDto>(created.Value);
            Assert.Equal((byte)1, returnDto.CurveId);

            var saved = await _context.CurvePoints.FindAsync(returnDto.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CreateCurvePoint_Should_Return_BadRequest_When_InvalidModel()
        {
            var dto = new CurvePointDto { CurveId = 0, Term = 2.0, CurvePointValue = 1.0 };
            _controller.ModelState.AddModelError("CurveId", "Le champ CurveId est requis.");

            var result = await _controller.CreateCurvePoint(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);
            Assert.True(modelState.ContainsKey("CurveId"));
        }

        [Fact]
        public async Task UpdateCurvePoint_Should_Return_Ok_With_Updated_Values()
        {
            var point = new CurvePoint { CurveId = 1, Term = 1, CurvePointValue = 0.5 };
            _context.CurvePoints.Add(point);
            _context.SaveChanges();

            var dto = new CurvePointDto { Id = point.Id, CurveId = 2, Term = 2, CurvePointValue = 5 };

            var result = await _controller.UpdateCurvePoint(point.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<CurvePointDto>(ok.Value);
            Assert.Equal((byte)2, returned.CurveId);
        }

        [Fact]
        public async Task UpdateCurvePoint_Should_Return_NotFound_When_NotExists()
        {
            var dto = new CurvePointDto { CurveId = 9, Term = 9, CurvePointValue = 9 };

            var result = await _controller.UpdateCurvePoint(-1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCurvePoint_Should_Return_NoContent_And_Remove()
        {
            var point = new CurvePoint { CurveId = 1, Term = 1, CurvePointValue = 1 };
            _context.CurvePoints.Add(point);
            _context.SaveChanges();

            var result = await _controller.DeleteCurvePoint(point.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.CurvePoints.FindAsync(point.Id));
        }

        [Fact]
        public async Task DeleteCurvePoint_Should_Return_NotFound_When_NotExists()
        {
            var result = await _controller.DeleteCurvePoint(-1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

