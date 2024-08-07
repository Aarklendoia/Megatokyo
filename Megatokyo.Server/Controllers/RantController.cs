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
    /// Create new RantsController instance.
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="mapper"></param>
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiController]
    public class RantsController(IMediator mediator, IMapper mapper) : ControllerBase
    {

        /// <summary>
        /// Get all rants.
        /// </summary>
        /// <returns>List of rants</returns>
        /// <response code="200">Return in case the list have some rants.</response>
        /// <response code="204">Return in case the list is empty.</response>
        /// <response code="500">Return in case of internal server error.</response> 
        [ProducesResponseType(typeof(List<RantOutputDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = nameof(GetAllRants))]
        public async Task<IActionResult> GetAllRants()
        {
            IEnumerable<Rant> rants = await mediator.Send(new GetAllRantsQuery());
            if (!rants.Any())
                return NoContent();
            IEnumerable<RantOutputDto> rantsOutputDTO = mapper.Map<IEnumerable<RantOutputDto>>(rants);
            return Ok(rantsOutputDTO);
        }

        /// <summary>
        /// Get a rant by his number.
        /// </summary>
        /// <param name="number">Rant's number</param>
        /// <returns>A rant</returns>
        /// <response code="200">Return in case the rant exists.</response>
        /// <response code="404">Returned in case the rant is not found.</response>*
        /// <response code="500">Return in case of internal server error.</response>
        [ProducesResponseType(typeof(RantOutputDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{number}", Name = nameof(GetRant))]
        public async Task<IActionResult> GetRant(int number)
        {
            Rant rant = await mediator.Send(new GetRantQuery(number));
            if (rant == default)
                return NotFound();
            return Ok(mapper.Map<RantOutputDto>(rant));
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
