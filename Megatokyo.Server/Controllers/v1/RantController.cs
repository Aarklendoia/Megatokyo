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
using System;

namespace Megatokyo.Server.Controllers.v1
{
    /// <summary>
    /// API for rants.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Create new RantsController instance.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="mapper"></param>
        public RantsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all rants.
        /// </summary>
        /// <returns>List of rants</returns>
        /// <response code="200">Return in case the list have some rants.</response>
        /// <response code="204">Return in case the list is empty.</response>
        /// <response code="500">Return in case of internal server error.</response> 
        [ProducesResponseType(typeof(List<RantOutputDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetAllRants))]
        public async Task<IActionResult> GetAllRants()
        {
            try
            {
                List<RantOutputDTO> rantsData = new();
                IEnumerable<RantDomain> rants = await _mediator.Send(new GetAllRantsQuery());
                if (!rants.Any())
                    return NoContent();
                foreach (RantDomain rant in rants)
                {
                    RantOutputDTO rantOutputDTO = _mapper.Map<RantOutputDTO>(rant);
                    rantsData.Add(rantOutputDTO);
                }
                return Ok(rantsData);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get a rant by his number.
        /// </summary>
        /// <param name="number">Rant's number</param>
        /// <returns>A rant</returns>
        /// <response code="200">Return in case the rant exists.</response>
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(RantOutputDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{number?}", Name = nameof(GetRants))]
        public async Task<IActionResult> GetRants(int number)
        {
            try
            {
                RantDomain rant = await _mediator.Send(new GetRantQuery(number));
                return Ok(rant);
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

    //private async Task<RantsTranslations> TranslateRant(int rantId, string language)
    //{
    //    string languageCode = new CultureInfo("en-US").TwoLetterISOLanguageName;
    //    try
    //    {
    //        CultureInfo cultureInfo = new CultureInfo(language);
    //        languageCode = cultureInfo.TwoLetterISOLanguageName;
    //    }
    //    catch (CultureNotFoundException e)
    //    {
    //        Debug.WriteLine(e.Message);
    //    }
    //    IEnumerable<RantsTranslations> rantsTranslations = await _repoWrapper.RantsTranslations.FindByConditionAsync(rt => rt.RantId == rantId && rt.Language == language);
    //    if (!rantsTranslations.Any())
    //    {
    //        IEnumerable<RantsTranslations> currentRantTranslation = await _repoWrapper.RantsTranslations.FindByConditionAsync(rt => rt.RantId == rantId && rt.Language == new CultureInfo("en-US").ToString());
    //        RantsTranslations currentTranslation = currentRantTranslation.First();
    //        string json = await _translator.Translate(languageCode, currentTranslation.Content);
    //        string translation = ExtractTranslationFromJson(json);
    //        RantsTranslations newRantsTranslations = new RantsTranslations
    //        {
    //            Language = language,
    //            RantId = rantId,
    //            Title = currentTranslation.Title,
    //            Content = translation
    //        };
    //        _repoWrapper.RantsTranslations.Create(newRantsTranslations);
    //        await _repoWrapper.RantsTranslations.SaveAsync();
    //        return newRantsTranslations;
    //    }
    //    return rantsTranslations.First();
    //}

    //private static string ExtractTranslationFromJson(string json)
    //{
    //    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TranslatorResult));
    //    using MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json.Trim(new char[] { '[', ']' })));
    //    TranslatorResult o = (TranslatorResult)jsonSerializer.ReadObject(memoryStream);
    //    Debug.WriteLine(o.Translations[0].Text);
    //    return o.Translations[0].Text;
    //}
}
