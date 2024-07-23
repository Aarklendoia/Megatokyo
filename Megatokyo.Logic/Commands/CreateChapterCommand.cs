using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class CreateCheckingCommand(Checking checkingToCreate) : IRequest<Checking>
    {
        public Checking CheckingToCreate { get; } = checkingToCreate;
    }

    public class CreateCheckingCommandHandler(ICheckingRepository checkingRepository) : IRequestHandler<CreateCheckingCommand, Checking>
    {
        public async Task<Checking> Handle(CreateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await checkingRepository.CreateAsync(request.CheckingToCreate);
        }
    }
}
