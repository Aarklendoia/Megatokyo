namespace Megatokyo.Server.Models.Syndication
{
    internal class Item
    {
        public string Link { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; } = DateTime.Today;
        public FeedType FeedType { get; set; } = FeedType.RSS;
    }
}
