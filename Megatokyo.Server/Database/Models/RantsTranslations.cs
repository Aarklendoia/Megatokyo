using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Server.Database.Models
{
    public partial class RantsTranslations
    {
        [Key]
        public int TranslationId { get; set; }
        [Required]
        public int RantId { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
