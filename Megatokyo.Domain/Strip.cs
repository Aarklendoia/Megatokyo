namespace Megatokyo.Domain
{
    public class Strip
    {
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public Uri? Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Category { get; set; } = string.Empty;

        public Strip(string category, int number, string title, Uri? url, DateTimeOffset publishDate)
        {
            Number = number;
            Title = title;
            Url = url;
            PublishDate = publishDate;
            Category = category;
        }

        public Strip(int number)
        {
            Number = number;
        }
    }
}
