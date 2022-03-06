namespace Megatokyo.Server.Models.Translations
{
    internal interface ITranslator
    {
        string ClientKey { get; set; }
        Task<string> Translate(string language, string text);
        Task<string> GetLanguages();
    }
}