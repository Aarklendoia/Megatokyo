using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetAllChaptersQuery : IRequest<IEnumerable<ChapterDomain>>
    {
        public GetAllChaptersQuery()
        {
        }
    }

    public class GetAllChaptersQueryHandler : IRequestHandler<GetAllChaptersQuery, IEnumerable<ChapterDomain>>
    {
        private readonly IEntitiesRepository _entityRepository;

        public GetAllChaptersQueryHandler(IEntitiesRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public async Task<IEnumerable<ChapterDomain>> Handle(GetAllChaptersQuery request, CancellationToken cancellationToken)
        {
            return await _entityRepository.Chapters.GetAllAsync();
        }
    }
}
