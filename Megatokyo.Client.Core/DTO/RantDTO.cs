using Newtonsoft.Json;
using System;

namespace Megatokyo.Client.Core.DTO
{
    internal class RantDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("number")]
        public int Number { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("publishDate")]
        public DateTimeOffset PublishDate { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
