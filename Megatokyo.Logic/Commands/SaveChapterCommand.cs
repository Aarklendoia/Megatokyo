using MediatR;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class SaveChapterCommand : IRequest<Unit>
    {
    }

    public class SaveChapterCommandHandler(IChapterRepository chapterRepository) : IRequestHandler<SaveChapterCommand, Unit>
    {
        async Task<Unit> IRequestHandler<SaveChapterCommand, Unit>.Handle(SaveChapterCommand request, CancellationToken cancellationToken)
        {
            await chapterRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
