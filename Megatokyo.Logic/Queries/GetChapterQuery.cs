using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetChapterQuery : IRequest<Chapter>
    {
        public string Category { get; set; }

        public GetChapterQuery(string category)
        {
            Category = category;
        }
    }

    public class GetChapterQueryHandler : IRequestHandler<GetChapterQuery, Chapter>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetChapterQueryHandler(IChapterRepository entityRepository)
        {
            _chapterRepository = entityRepository;
        }

        public async Task<Chapter> Handle(GetChapterQuery request, CancellationToken cancellationToken)
        {
            return await _chapterRepository.GetAsync(request.Category);
        }
    }
}
