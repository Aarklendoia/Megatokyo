using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Megatokyo.Server.Models.Translations
{
    [DataContract]
    internal class DetectedLanguage
    {
        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "score")]
        public double Score { get; set; }
    }

    [DataContract]
    internal class Translation
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }
    }

    [DataContract]
    internal class TranslatorResult
    {
        [DataMember(Name = "detectedLanguage")]
        public DetectedLanguage DetectedLanguage { get; set; }

        [DataMember(Name = "translations")]
        public IList<Translation> Translations { get; set; }
    }

}
