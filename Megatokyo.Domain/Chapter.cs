namespace Megatokyo.Domain
{
    public class Chapter
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }

        public Chapter(int number, string title, string category)
        {
            Number = number;
            Title = title;
            Category = category;
        }
    }
}
