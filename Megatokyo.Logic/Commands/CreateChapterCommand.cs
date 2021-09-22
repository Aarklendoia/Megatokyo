using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class CreateCheckingCommand : IRequest<Checking>
    {
        public Checking CheckingToCreate { get; }

        public CreateCheckingCommand(Checking checkingToCreate)
        {
            CheckingToCreate = checkingToCreate;
        }
    }

    public class CreateCheckingCommandHandler : IRequestHandler<CreateCheckingCommand, Checking>
    {
        private readonly ICheckingRepository _checkingRepository;

        public CreateCheckingCommandHandler(ICheckingRepository checkingRepository)
        {
            _checkingRepository = checkingRepository;
        }

        public async Task<Checking> Handle(CreateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.CreateAsync(request.CheckingToCreate);
        }
    }
}
