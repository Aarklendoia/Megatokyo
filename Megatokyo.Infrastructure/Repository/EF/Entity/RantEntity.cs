using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class RantEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public int Number { get; set; }
        [Required]
        public string Author { get; set; } = null!;
        [Required]
        public Uri Url { get; set; } = null!;
        [Required]
        public DateTimeOffset PublishDate { get; set; }
        [Required]
        public string Content { get; set; } = null!;
    }
}
