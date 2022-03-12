using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllChaptersQuery : IRequest<IEnumerable<Chapter>>
    {
        public GetAllChaptersQuery()
        {
        }
    }

    public class GetAllChaptersQueryHandler : IRequestHandler<GetAllChaptersQuery, IEnumerable<Chapter>>
    {
        private readonly IChapterRepository _chapterRepository;

        public GetAllChaptersQueryHandler(IChapterRepository entityRepository)
        {
            _chapterRepository = entityRepository;
        }

        public async Task<IEnumerable<Chapter>> Handle(GetAllChaptersQuery request, CancellationToken cancellationToken)
        {
            return await _chapterRepository.GetAllAsync();
        }
    }
}
