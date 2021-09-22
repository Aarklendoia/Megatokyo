using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetCheckingQuery : IRequest<Checking>
    {
        public int Number { get; set; }

        public GetCheckingQuery(int number)
        {
            Number = number;
        }
    }

    public class GetCheckingQueryHandler : IRequestHandler<GetCheckingQuery, Checking>
    {
        private readonly ICheckingRepository _checkingRepository;

        public GetCheckingQueryHandler(ICheckingRepository entityRepository)
        {
            _checkingRepository = entityRepository;
        }

        public async Task<Checking> Handle(GetCheckingQuery request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.GetAsync(request.Number);
        }
    }
}
