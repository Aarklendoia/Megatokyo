namespace Megatokyo.Server.Database.Contracts
{
    public interface IRepositoryWrapper
    {
        IChaptersRepository Chapters { get; }
        IStripsRepository Strips { get; }
        IRantsRepository Rants { get; }
        IRantsTranslationsRepository RantsTranslations { get; }
        ICheckingRepository Checking { get; }
    }
}
