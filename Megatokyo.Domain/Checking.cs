namespace Megatokyo.Domain
{
    public class Checking
    {
        public int Id { get; set; } = 0;
        public DateTimeOffset LastCheck { get; set; } = DateTimeOffset.MinValue;
        public int LastStripNumber { get; set; } = 0;
        public int LastRantNumber { get; set; } = 0;
    }
}
