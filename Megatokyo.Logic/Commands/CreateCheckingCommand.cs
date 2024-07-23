using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class CreateChapterCommand(Chapter chapterToCreate) : IRequest<Chapter>
    {
        public Chapter ChapterToCreate { get; } = chapterToCreate;
    }

    public class CreateChapterCommandHandler(IChapterRepository chapterRepository) : IRequestHandler<CreateChapterCommand, Chapter>
    {
        public async Task<Chapter> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
        {
            return await chapterRepository.CreateAsync(request.ChapterToCreate);
        }
    }
}
