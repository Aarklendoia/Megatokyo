using Megatokyo.Server.Models.Translations;
using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Megatokyo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RantsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly ITranslator _translator;

        public string ClientKey { get; set; }

        public RantsController(IRepositoryWrapper repoWrapper, ITranslator translator, IOptions<TranslatorSettings> settingsOptions)
        {
            if (settingsOptions == null)
            {
                throw new ArgumentNullException(nameof(settingsOptions));
            }

            TranslatorSettings translatorSettings = settingsOptions.Value;
            _repoWrapper = repoWrapper ?? throw new ArgumentNullException(nameof(repoWrapper));
            _translator = translator ?? throw new ArgumentNullException(nameof(translator));
            _translator.ClientKey = translatorSettings.ClientKey;
        }

        [HttpGet]
        public async Task<IEnumerable<Rants>> GetRants()
        {
            return await _repoWrapper.Rants.FindAllAsync().ConfigureAwait(false);
        }

        [HttpGet("{number}/{language}")]
        public async Task<ActionResult<Rants>> GetRants([FromRoute] int number, string language)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<Rants> rants = await _repoWrapper.Rants.FindByConditionAsync(r => r.Number == number).ConfigureAwait(false);
            Rants rant = rants.First();
            IEnumerable<RantsTranslations> rantsTranslations = await _repoWrapper.RantsTranslations.FindByConditionAsync(rt => rt.RantId == rant.RantId && (rt.Language == language || rt.Language == new CultureInfo("en-US").ToString())).ConfigureAwait(false);
            //if (rant.RantsTranslations.Count == 1)
            //{
            //    RantsTranslations newRantsTranslations = await TranslateRant(rant.RantId, language).ConfigureAwait(false);
            //    rant.RantsTranslations.Add(newRantsTranslations);
            //}
            //else
            //{
            //    rant.AddTranslations(rantsTranslations.ToList());
            //}

            if (rant == null)
            {
                return NotFound();
            }

            return Ok(rant);
        }

        private async Task<RantsTranslations> TranslateRant(int rantId, string language)
        {
            string languageCode = new CultureInfo("en-US").TwoLetterISOLanguageName;
            try
            {
                CultureInfo cultureInfo = new CultureInfo(language);
                languageCode = cultureInfo.TwoLetterISOLanguageName;
            }
            catch (CultureNotFoundException e)
            {
                Debug.WriteLine(e.Message);
            }
            IEnumerable<RantsTranslations> rantsTranslations = await _repoWrapper.RantsTranslations.FindByConditionAsync(rt => rt.RantId == rantId && rt.Language == language).ConfigureAwait(false);
            if (!rantsTranslations.Any())
            {
                IEnumerable<RantsTranslations> currentRantTranslation = await _repoWrapper.RantsTranslations.FindByConditionAsync(rt => rt.RantId == rantId && rt.Language == new CultureInfo("en-US").ToString()).ConfigureAwait(false);
                RantsTranslations currentTranslation = currentRantTranslation.First();
                string json = await _translator.Translate(languageCode, currentTranslation.Content).ConfigureAwait(false);
                string translation = ExtractTranslationFromJson(json);
                RantsTranslations newRantsTranslations = new RantsTranslations
                {
                    Language = language,
                    RantId = rantId,
                    Title = currentTranslation.Title,
                    Content = translation
                };
                _repoWrapper.RantsTranslations.Create(newRantsTranslations);
                await _repoWrapper.RantsTranslations.SaveAsync().ConfigureAwait(false);
                return newRantsTranslations;
            }
            return rantsTranslations.First();
        }

        private static string ExtractTranslationFromJson(string json)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TranslatorResult));
            using MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(json.Trim(new char[] { '[', ']' })));
            TranslatorResult o = (TranslatorResult)jsonSerializer.ReadObject(memoryStream);
            Debug.WriteLine(o.Translations[0].Text);
            return o.Translations[0].Text;
        }
    }
}