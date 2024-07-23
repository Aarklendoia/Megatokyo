namespace Megatokyo.Domain
{
    public class Strip
    {
        public int Number { get; set; } = 0;
        public string Title { get; set; } = string.Empty;
        public Uri? Url { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.MinValue;
        public string Category { get; set; } = string.Empty;
    }
}
