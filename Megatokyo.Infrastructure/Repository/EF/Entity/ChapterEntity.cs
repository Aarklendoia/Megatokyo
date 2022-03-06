using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class ChapterEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public int Number { get; set; }
        [Required]
        public string Category { get; set; } = null!;
    }
}
