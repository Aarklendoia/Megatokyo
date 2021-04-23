﻿using AutoMapper;
using MediatR;
using Megatokyo.Domain;
using Megatokyo.Server.DTO.v1;
using Megatokyo.Logic.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Controllers.v1
{
    /// <summary>
    /// API for chapters.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Create new ChaptersController instance.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="mapper"></param>
        public ChaptersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all chapters.
        /// </summary>
        /// <returns>List of chapters</returns>
        /// <response code="200">Return in case the list have some chapters.</response>
        /// <response code="204">Return in case the list is empty.</response>
        /// <response code="500">Return in case of internal server error.</response> 
        [ProducesResponseType(typeof(List<ChapterOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetAllChapters))]
        public async Task<IActionResult> GetAllChapters()
        {
            try
            {
                List<ChapterOutputDTO> ChaptersData = new();
                IEnumerable<ChapterDomain> chapters = await _mediator.Send(new GetAllChaptersQuery());
                if (!chapters.Any())
                    return NoContent();
                foreach (ChapterDomain chapter in chapters)
                {
                    ChapterOutputDTO ChapterOutputDTO = _mapper.Map<ChapterOutputDTO>(chapter);
                    ChaptersData.Add(ChapterOutputDTO);
                }
                return Ok(ChaptersData);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get a chapter by his number.
        /// </summary>
        /// <param name="number">Chapter's number</param>
        /// <returns>A Chapter</returns>
        /// <response code="200">Return in case the chapter exists.</response>
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(ChapterOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{number?}", Name = nameof(GetChapters))]
        public async Task<IActionResult> GetChapters(int number)
        {
            try
            {
                ChapterDomain Chapter = await _mediator.Send(new GetChapterQuery(number));
                return Ok(Chapter);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
