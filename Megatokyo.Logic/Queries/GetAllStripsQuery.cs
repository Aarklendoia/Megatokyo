using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetAllStripsQuery : IRequest<IEnumerable<Strip>>
    {
    }

    public class GetAllStripsQueryHandler(IStripRepository entityRepository) : IRequestHandler<GetAllStripsQuery, IEnumerable<Strip>>
    {
        public async Task<IEnumerable<Strip>> Handle(GetAllStripsQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAllAsync();
        }
    }
}
