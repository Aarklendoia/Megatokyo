using Xunit;
using Shouldly;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Controllers;
using Megatokyo.Server.Dto.v1;

namespace Megatokyo.Server.UnitTest
{
    public class RantsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RantsController _controller;

        public RantsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new RantsController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllRants_ReturnsOkResult_WhenRantsExist()
        {
            // Arrange
            var rants = new List<Rant> { new(), new() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRantsQuery>(), default)).ReturnsAsync(rants);
            _mapperMock.Setup(m => m.Map<IEnumerable<RantOutputDto>>(rants)).Returns([new RantOutputDto(), new RantOutputDto()]);

            // Act
            var result = await _controller.GetAllRants();

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnRants = okResult.Value.ShouldBeAssignableTo<IEnumerable<RantOutputDto>>();
            returnRants.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetAllRants_ReturnsNoContent_WhenNoRantsExist()
        {
            // Arrange
            var rants = new List<Rant>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRantsQuery>(), default)).ReturnsAsync(rants);

            // Act
            var result = await _controller.GetAllRants();

            // Assert
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetRant_ReturnsOkResult_WhenRantExists()
        {
            // Arrange
            var rant = new Rant();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRantQuery>(), default)).ReturnsAsync(rant);
            _mapperMock.Setup(m => m.Map<RantOutputDto>(rant)).Returns(new RantOutputDto());

            // Act
            var result = await _controller.GetRant(1);

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnRant = okResult.Value.ShouldBeAssignableTo<RantOutputDto>();
            returnRant.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetRant_ReturnsNotFound_WhenRantDoesNotExist()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRantQuery>(), default)).ReturnsAsync((Rant)null!);

            // Act
            var result = await _controller.GetRant(1);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
