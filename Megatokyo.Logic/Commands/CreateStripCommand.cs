using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateStripCommand : IRequest<StripDomain>
    {
        public StripDomain StripToCreate { get; }

        public CreateStripCommand(StripDomain stripToCreate)
        {
            StripToCreate = stripToCreate;
        }
    }

    public class CreateStripCommandHandler : IRequestHandler<CreateStripCommand, StripDomain>
    {
        private readonly IStripRepository _stripRepository;

        public CreateStripCommandHandler(IStripRepository stripRepository)
        {
            _stripRepository = stripRepository;
        }

        public async Task<StripDomain> Handle(CreateStripCommand request, CancellationToken cancellationToken)
        {
            return await _stripRepository.CreateAsync(request.StripToCreate);
        }
    }
}
