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
    public class ChaptersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ChaptersController _controller;

        public ChaptersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ChaptersController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllChapters_ReturnsOkResult_WhenChaptersExist()
        {
            // Arrange
            var chapters = new List<Chapter> { new(), new() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllChaptersQuery>(), default)).ReturnsAsync(chapters);
            _mapperMock.Setup(m => m.Map<IEnumerable<ChapterOutputDto>>(chapters)).Returns([new ChapterOutputDto(), new ChapterOutputDto()]);

            // Act
            var result = await _controller.GetAllChapters();

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnChapters = okResult.Value.ShouldBeAssignableTo<IEnumerable<ChapterOutputDto>>();
            returnChapters.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetAllChapters_ReturnsNoContent_WhenNoChaptersExist()
        {
            // Arrange
            var chapters = new List<Chapter>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllChaptersQuery>(), default)).ReturnsAsync(chapters);

            // Act
            var result = await _controller.GetAllChapters();

            // Assert
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetChapter_ReturnsOkResult_WhenChapterExists()
        {
            // Arrange
            var chapter = new Chapter();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChapterQuery>(), default)).ReturnsAsync(chapter);
            _mapperMock.Setup(m => m.Map<ChapterOutputDto>(chapter)).Returns(new ChapterOutputDto());

            // Act
            var result = await _controller.GetChapter("some-category");

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnChapter = okResult.Value.ShouldBeAssignableTo<ChapterOutputDto>();
            returnChapter.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetChapter_ReturnsNotFound_WhenChapterDoesNotExist()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetChapterQuery>(), default)).ReturnsAsync((Chapter)null!);

            // Act
            var result = await _controller.GetChapter("some-category");

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }
    }
}
