using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllChaptersQuery : IRequest<IEnumerable<Chapter>>
    {
    }

    public class GetAllChaptersQueryHandler(IChapterRepository entityRepository) : IRequestHandler<GetAllChaptersQuery, IEnumerable<Chapter>>
    {
        public async Task<IEnumerable<Chapter>> Handle(GetAllChaptersQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAllAsync();
        }
    }
}
