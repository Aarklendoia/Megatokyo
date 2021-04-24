using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetAllStripsQuery : IRequest<IEnumerable<StripDomain>>
    {
        public GetAllStripsQuery()
        {
        }
    }

    public class GetAllStripsQueryHandler : IRequestHandler<GetAllStripsQuery, IEnumerable<StripDomain>>
    {
        private readonly IStripRepository _stripRepository;

        public GetAllStripsQueryHandler(IStripRepository entityRepository)
        {
            _stripRepository = entityRepository;
        }

        public async Task<IEnumerable<StripDomain>> Handle(GetAllStripsQuery request, CancellationToken cancellationToken)
        {
            return await _stripRepository.GetAllAsync();
        }
    }
}
