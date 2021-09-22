using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateStripCommand : IRequest<Strip>
    {
        public Strip StripToCreate { get; }

        public CreateStripCommand(Strip stripToCreate)
        {
            StripToCreate = stripToCreate;
        }
    }

    public class CreateStripCommandHandler : IRequestHandler<CreateStripCommand, Strip>
    {
        private readonly IStripRepository _stripRepository;

        public CreateStripCommandHandler(IStripRepository stripRepository)
        {
            _stripRepository = stripRepository;
        }

        public async Task<Strip> Handle(CreateStripCommand request, CancellationToken cancellationToken)
        {
            return await _stripRepository.CreateAsync(request.StripToCreate);
        }
    }
}
