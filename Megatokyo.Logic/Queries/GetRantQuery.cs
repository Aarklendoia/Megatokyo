using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetRantQuery : IRequest<Rant>
    {
        public int Number { get; set; }

        public GetRantQuery(int number)
        {
            Number = number;
        }
    }

    public class GetRantQueryHandler : IRequestHandler<GetRantQuery, Rant>
    {
        private readonly IRantRepository _rantRepository;

        public GetRantQueryHandler(IRantRepository entityRepository)
        {
            _rantRepository = entityRepository;
        }

        public async Task<Rant> Handle(GetRantQuery request, CancellationToken cancellationToken)
        {
            return await _rantRepository.GetAsync(request.Number);
        }
    }
}
