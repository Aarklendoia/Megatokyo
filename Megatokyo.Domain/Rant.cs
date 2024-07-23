namespace Megatokyo.Domain
{
    public class Rant
    {
        public string Title { get; set; } = string.Empty;
        public int Number { get; set; } = 0;
        public string Author { get; set; } = string.Empty;
        public Uri? Url { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.MinValue;
        public string Content { get; set; } = string.Empty;
    }
}
