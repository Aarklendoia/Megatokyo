using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllRantsQuery : IRequest<IEnumerable<Rant>>
    {
        public GetAllRantsQuery()
        {
        }
    }

    public class GetAllRantsQueryHandler : IRequestHandler<GetAllRantsQuery, IEnumerable<Rant>>
    {
        private readonly IRantRepository _rantRepository;

        public GetAllRantsQueryHandler(IRantRepository entityRepository)
        {
            _rantRepository = entityRepository;
        }

        public async Task<IEnumerable<Rant>> Handle(GetAllRantsQuery request, CancellationToken cancellationToken)
        {
            return await _rantRepository.GetAllAsync();
        }
    }
}
