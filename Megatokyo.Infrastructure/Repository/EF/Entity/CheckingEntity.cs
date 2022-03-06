using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class CheckingEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTimeOffset LastCheck { get; set; }
        [Required]
        public int LastStripNumber { get; set; }
        [Required]
        public int LastRantNumber { get; set; }
    }
}
