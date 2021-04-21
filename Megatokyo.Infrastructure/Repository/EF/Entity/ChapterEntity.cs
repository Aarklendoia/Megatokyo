using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class ChapterEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Category { get; set; }
    }
}
