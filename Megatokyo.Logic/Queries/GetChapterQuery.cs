using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetChapterQuery : IRequest<ChapterDomain>
    {
        public string Category { get; set; }

        public GetChapterQuery(string category)
        {
            Category = category;
        }
    }

    public class GetChapterQueryHandler : IRequestHandler<GetChapterQuery, ChapterDomain>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChapterQueryHandler(IChapterRepository entityRepository)
        {
            _chapterRepository = entityRepository;
        }

        public async Task<ChapterDomain> Handle(GetChapterQuery request, CancellationToken cancellationToken)
        {
            return await _chapterRepository.GetAsync(request.Category);
        }
    }
}
