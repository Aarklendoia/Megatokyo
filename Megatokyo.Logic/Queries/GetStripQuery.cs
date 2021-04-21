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
        private readonly IStripRepository _stripRepository;

        public GetStripQueryHandler(IStripRepository entityRepository)
        {
            _stripRepository = entityRepository;
        }

        public async Task<StripDomain> Handle(GetStripQuery request, CancellationToken cancellationToken)
        {
            return await _stripRepository.GetAsync(request.Number);
        }
    }
}
