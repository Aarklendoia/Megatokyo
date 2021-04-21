using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetStripQuery : IRequest<StripDomain>
    {
        public int Number { get; set; }

        public GetStripQuery(int number)
        {
            Number = number;
        }
    }

    public class GetStripQueryHandler : IRequestHandler<GetStripQuery, StripDomain>
    {
        private readonly IEntitiesRepository _entityRepository;

        public GetStripQueryHandler(IEntitiesRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public async Task<StripDomain> Handle(GetStripQuery request, CancellationToken cancellationToken)
        {
            return await _entityRepository.Strips.FindAsync(request.Number);
        }
    }
}
