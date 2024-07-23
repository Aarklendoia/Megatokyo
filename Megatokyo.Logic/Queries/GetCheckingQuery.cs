using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetCheckingQuery(int number) : IRequest<Checking>
    {
        public int Number { get; set; } = number;
    }

    public class GetCheckingQueryHandler(ICheckingRepository entityRepository) : IRequestHandler<GetCheckingQuery, Checking>
    {
        public async Task<Checking> Handle(GetCheckingQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAsync(request.Number);
        }
    }
}
