using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetRantQuery(int number) : IRequest<Rant>
    {
        public int Number { get; set; } = number;
    }

    public class GetRantQueryHandler(IRantRepository entityRepository) : IRequestHandler<GetRantQuery, Rant>
    {
        public async Task<Rant> Handle(GetRantQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAsync(request.Number);
        }
    }
}
