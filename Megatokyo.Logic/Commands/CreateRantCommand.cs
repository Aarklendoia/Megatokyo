using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateRantCommand : IRequest<RantDomain>
    {
        public RantDomain RantToCreate { get; }

        public CreateRantCommand(RantDomain rantToCreate)
        {
            RantToCreate = rantToCreate;
        }
    }

    public class CreateRantCommandHandler : IRequestHandler<CreateRantCommand, RantDomain>
    {
        private readonly IRantRepository _rantRepository;

        public CreateRantCommandHandler(IRantRepository rantRepository)
        {
            _rantRepository = rantRepository;
        }

        public async Task<RantDomain> Handle(CreateRantCommand request, CancellationToken cancellationToken)
        {
            return await _rantRepository.CreateAsync(request.RantToCreate);
        }
    }
}
