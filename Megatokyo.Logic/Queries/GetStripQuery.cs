using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetStripQuery(int number) : IRequest<Strip>
    {
        public int Number { get; set; } = number;
    }

    public class GetStripQueryHandler(IStripRepository entityRepository) : IRequestHandler<GetStripQuery, Strip>
    {
        public async Task<Strip> Handle(GetStripQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAsync(request.Number);
        }
    }
}
