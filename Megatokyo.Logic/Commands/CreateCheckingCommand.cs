using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateChapterCommand : IRequest<Chapter>
    {
        public Chapter ChapterToCreate { get; }

        public CreateChapterCommand(Chapter chapterToCreate)
        {
            ChapterToCreate = chapterToCreate;
        }
    }

    public class CreateChapterCommandHandler : IRequestHandler<CreateChapterCommand, Chapter>
    {
        private readonly IChapterRepository _chapterRepository;

        public CreateChapterCommandHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<Chapter> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
        {
            return await _chapterRepository.CreateAsync(request.ChapterToCreate);
        }
    }
}
