using AutoMapper;
using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Queries;
using Megatokyo.Server.DTO.v1;
using Microsoft.AspNetCore.Mvc;

namespace Megatokyo.Server.Controllers.v1
{
    /// <summary>
    /// API for strips.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class StripsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Create new StripsController instance.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="mapper"></param>
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
            IEnumerable<Strip> strips = await _mediator.Send(new GetAllStripsQuery());
            if (!strips.Any())
                return NoContent();
            IEnumerable<StripOutputDTO> stripsOutputDTO = _mapper.Map<IEnumerable<StripOutputDTO>>(strips);
            return Ok(stripsOutputDTO);
        }

        /// <summary>
        /// Get this strip of a category.
        /// </summary>
        /// <param name="category">Strip's category</param>
        /// <returns>A strip</returns>
        /// <response code="200">Return in case the strip exists.</response>
        /// <response code="400">Return in case the parameters are incorect.</response>
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(List<StripOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("category/{category}", Name = nameof(GetCategoryStrips))]
        public async Task<IActionResult> GetCategoryStrips(string category)
        {
            IEnumerable<Strip> strips = await _mediator.Send(new GetCategoryStripsQuery(category));
            if (!strips.Any())
                return NoContent();
            IEnumerable<StripOutputDTO> stripsOutputDTO = _mapper.Map<IEnumerable<StripOutputDTO>>(strips);
            return Ok(stripsOutputDTO);
        }

        /// <summary>
        /// Get a strip by his number.
        /// </summary>
        /// <param name="number">Strip's number</param>
        /// <returns>A strip</returns>
        /// <response code="200">Return in case the strip exists.</response>
        /// <response code="400">Return in case the parameters are incorect.</response>*
        /// <response code="404">Returned in case the strip is not found.</response>*
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(StripOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{number}", Name = nameof(GetStrip))]
        public async Task<IActionResult> GetStrip(int number)
        {
            Strip strip = await _mediator.Send(new GetStripQuery(number));
            if (strip == default)
                return NotFound();
            StripOutputDTO stripOutputDTO = _mapper.Map<StripOutputDTO>(strip);
            return Ok(stripOutputDTO);
        }
    }
}