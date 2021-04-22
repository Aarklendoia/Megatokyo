using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateChapterCommand : IRequest<ChapterDomain>
    {
        public ChapterDomain ChapterToCreate { get; }

        public CreateChapterCommand(ChapterDomain chapterToCreate)
        {
            ChapterToCreate = chapterToCreate;
        }
    }

    public class CreateChapterCommandHandler : IRequestHandler<CreateChapterCommand, ChapterDomain>
    {
        private readonly IChapterRepository _chapterRepository;

        public CreateChapterCommandHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<ChapterDomain> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
        {
            return await _chapterRepository.CreateAsync(request.ChapterToCreate);
        }
    }
}
