using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateCheckingCommand : IRequest<CheckingDomain>
    {
        public CheckingDomain CheckingToCreate { get; }

        public CreateCheckingCommand(CheckingDomain checkingToCreate)
        {
            CheckingToCreate = checkingToCreate;
        }
    }

    public class CreateCheckingCommandHandler : IRequestHandler<CreateCheckingCommand, CheckingDomain>
    {
        private readonly ICheckingRepository _checkingRepository;

        public CreateCheckingCommandHandler(ICheckingRepository checkingRepository)
        {
            _checkingRepository = checkingRepository;
        }

        public async Task<CheckingDomain> Handle(CreateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.CreateAsync(request.CheckingToCreate);
        }
    }
}
