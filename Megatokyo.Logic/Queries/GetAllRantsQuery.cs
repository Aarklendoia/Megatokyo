using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetAllRantsQuery : IRequest<IEnumerable<RantDomain>>
    {
        public GetAllRantsQuery()
        {
        }
    }

    public class GetAllRantsQueryHandler : IRequestHandler<GetAllRantsQuery, IEnumerable<RantDomain>>
    {
        private readonly IRantRepository _rantRepository;

        public GetAllRantsQueryHandler(IRantRepository entityRepository)
        {
            _rantRepository = entityRepository;
        }

        public async Task<IEnumerable<RantDomain>> Handle(GetAllRantsQuery request, CancellationToken cancellationToken)
        {
            return await _rantRepository.GetAllAsync();
        }
    }
}
