﻿using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Megatokyo.Server.DTO.v1;
using AutoMapper;
using MediatR;
using Megatokyo.Logic.Queries;
using Megatokyo.Domain;

namespace Megatokyo.Server.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public StripsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all strips.
        /// </summary>
        /// <returns>List of strips</returns>
        /// <response code="200">Return in case the list have some strips.</response>
        /// <response code="204">Return in case the list is empty.</response>
        /// <response code="500">Return in case of internal server error.</response> 
        [ProducesResponseType(typeof(List<StripOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetAllStrips))]
        public async Task<IActionResult> GetAllStrips()
        {
            List<StripOutputDTO> stripsData = new();
            IEnumerable<StripDomain> strips = await _mediator.Send(new GetAllStripsQuery());
            if (!strips.Any())
                return NoContent();
            foreach (StripDomain strip in strips)
            {
                StripOutputDTO stripOutputDTO = _mapper.Map<StripOutputDTO>(strip);
                stripsData.Add(stripOutputDTO);
            }
            return Ok(stripsData);
        }

        /// <summary>
        /// Get a strip by his number.
        /// </summary>
        /// <param name="number">Strip's number</param>
        /// <returns>A strip</returns>
        /// <response code="200">Return in case the strip exists.</response>
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(StripOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{number?}", Name = nameof(GetStrips))]
        public async Task<IActionResult> GetStrips(int number)
        {
            StripDomain strip = await _mediator.Send(new GetStripQuery(number));

            return Ok(strip);
        }
    }
}