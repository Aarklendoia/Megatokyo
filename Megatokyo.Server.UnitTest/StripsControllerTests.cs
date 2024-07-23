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
using Megatokyo.Server.Controllers.v1;
using Megatokyo.Server.Dto.v1;

namespace Megatokyo.Server.UnitTest
{
    public class StripsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StripsController _controller;

        public StripsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new StripsController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllStrips_ReturnsOkResult_WhenStripsExist()
        {
            // Arrange
            var strips = new List<Strip> { new Strip(), new Strip() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllStripsQuery>(), default)).ReturnsAsync(strips);
            _mapperMock.Setup(m => m.Map<IEnumerable<StripOutputDto>>(strips)).Returns([new StripOutputDto(), new StripOutputDto()]);

            // Act
            var result = await _controller.GetAllStrips();

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnStrips = okResult.Value.ShouldBeAssignableTo<IEnumerable<StripOutputDto>>();
            returnStrips.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetAllStrips_ReturnsNoContent_WhenNoStripsExist()
        {
            // Arrange
            var strips = new List<Strip>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllStripsQuery>(), default)).ReturnsAsync(strips);

            // Act
            var result = await _controller.GetAllStrips();

            // Assert
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetCategoryStrips_ReturnsOkResult_WhenStripsExist()
        {
            // Arrange
            var strips = new List<Strip> { new(), new() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryStripsQuery>(), default)).ReturnsAsync(strips);
            _mapperMock.Setup(m => m.Map<IEnumerable<StripOutputDto>>(strips)).Returns(new List<StripOutputDto> { new StripOutputDto(), new StripOutputDto() });

            // Act
            var result = await _controller.GetCategoryStrips("some-category");

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnStrips = okResult.Value.ShouldBeAssignableTo<IEnumerable<StripOutputDto>>();
            returnStrips.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetCategoryStrips_ReturnsNoContent_WhenNoStripsExist()
        {
            // Arrange
            var strips = new List<Strip>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryStripsQuery>(), default)).ReturnsAsync(strips);

            // Act
            var result = await _controller.GetCategoryStrips("some-category");

            // Assert
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetStrip_ReturnsOkResult_WhenStripExists()
        {
            // Arrange
            var strip = new Strip();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStripQuery>(), default)).ReturnsAsync(strip);
            _mapperMock.Setup(m => m.Map<StripOutputDto>(strip)).Returns(new StripOutputDto());

            // Act
            var result = await _controller.GetStrip(1);

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnStrip = okResult.Value.ShouldBeAssignableTo<StripOutputDto>();
            returnStrip.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetStrip_ReturnsNotFound_WhenStripDoesNotExist()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStripQuery>(), default)).ReturnsAsync((Strip)null!);

            // Act
            var result = await _controller.GetStrip(1);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
