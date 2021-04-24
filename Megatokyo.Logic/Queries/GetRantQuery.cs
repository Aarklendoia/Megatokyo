using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetRantQuery : IRequest<RantDomain>
    {
        public int Number { get; set; }

        public GetRantQuery(int number)
        {
            Number = number;
        }
    }

    public class GetRantQueryHandler : IRequestHandler<GetRantQuery, RantDomain>
    {
        private readonly IRantRepository _rantRepository;

        public GetRantQueryHandler(IRantRepository entityRepository)
        {
            _rantRepository = entityRepository;
        }

        public async Task<RantDomain> Handle(GetRantQuery request, CancellationToken cancellationToken)
        {
            return await _rantRepository.GetAsync(request.Number);
        }
    }
}
