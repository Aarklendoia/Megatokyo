using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllRantsQuery : IRequest<IEnumerable<Rant>>
    {
    }

    public class GetAllRantsQueryHandler(IRantRepository entityRepository) : IRequestHandler<GetAllRantsQuery, IEnumerable<Rant>>
    {
        public async Task<IEnumerable<Rant>> Handle(GetAllRantsQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAllAsync();
        }
    }
}
