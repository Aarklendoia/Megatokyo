using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllStripsQuery : IRequest<IEnumerable<Strip>>
    {
        public GetAllStripsQuery()
        {
        }
    }

    public class GetAllStripsQueryHandler : IRequestHandler<GetAllStripsQuery, IEnumerable<Strip>>
    {
        private readonly IStripRepository _stripRepository;

        public GetAllStripsQueryHandler(IStripRepository entityRepository)
        {
            _stripRepository = entityRepository;
        }

        public async Task<IEnumerable<Strip>> Handle(GetAllStripsQuery request, CancellationToken cancellationToken)
        {
            return await _stripRepository.GetAllAsync();
        }
    }
}
