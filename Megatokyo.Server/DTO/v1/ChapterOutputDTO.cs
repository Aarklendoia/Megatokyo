namespace Megatokyo.Server.Dto.v1
{
    internal class ChapterOutputDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = String.Empty;
    }
}