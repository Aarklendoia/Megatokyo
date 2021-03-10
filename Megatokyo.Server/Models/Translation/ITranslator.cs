﻿using System.Threading.Tasks;

namespace Megatokyo.Server.Models.Translations
{
    public interface ITranslator
    {
        string ClientKey { get; set; }
        Task<string> Translate(string language, string text);
        Task<string> GetLanguages();
    }
}