using MediatR;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class SaveChapterCommand : IRequest<Unit>
    {
        public SaveChapterCommand()
        {
        }
    }

    public class SaveChapterCommandHandler : IRequestHandler<SaveChapterCommand, Unit>
    {
        private readonly IChapterRepository _chapterRepository;

        public SaveChapterCommandHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        async Task<Unit> IRequestHandler<SaveChapterCommand, Unit>.Handle(SaveChapterCommand request, CancellationToken cancellationToken)
        {
            await _chapterRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
