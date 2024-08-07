﻿using Asp.Versioning;
using AutoMapper;
using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.Dto.v1;
using Microsoft.AspNetCore.Mvc;

namespace Megatokyo.Server.Controllers
{
    /// <summary>
    /// API for chapters.
    /// </summary>
    /// <remarks>
    /// Create new ChaptersController instance.
    /// </remarks>
    /// <param name="mediator"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class ChaptersController(IMediator mediator, IMapper mapper) : ControllerBase
    {

        /// <summary>
        /// Get all chapters.
        /// </summary>
        /// <returns>List of chapters</returns>
        /// <response code="200">Return in case the list have some chapters.</response>
        /// <response code="204">Return in case the list is empty.</response>
        /// <response code="500">Return in case of internal server error.</response> 
        [ProducesResponseType(typeof(List<ChapterOutputDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetAllChapters))]
        public async Task<IActionResult> GetAllChapters()
        {
            IEnumerable<Chapter> chapters = await mediator.Send(new GetAllChaptersQuery());
            if (!chapters.Any())
                return NoContent();
            IEnumerable<ChapterOutputDto> chaptersOutputDTO = mapper.Map<IEnumerable<ChapterOutputDto>>(chapters);
            return Ok(chaptersOutputDTO);
        }

        /// <summary>
        /// Get a chapter by his number.
        /// </summary>
        /// <param name="category">Chapter's category</param>
        /// <returns>A Chapter</returns>
        /// <response code="200">Return in case the chapter exists.</response>
        /// <response code="400">Return in case the parameters are incorect.</response>
        /// <response code="404">Returned in case the chapter is not found.</response>*
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(ChapterOutputDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{category}", Name = nameof(GetChapter))]
        public async Task<IActionResult> GetChapter(string category)
        {
            Chapter ChapterData = await mediator.Send(new GetChapterQuery(category));
            ChapterOutputDto chapter = mapper.Map<ChapterOutputDto>(ChapterData);
            if (chapter == default)
                return NotFound();
            return Ok(chapter);
        }
    }
}
