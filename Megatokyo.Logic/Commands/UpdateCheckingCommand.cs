using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class UpdateCheckingCommand(Checking checkingToUpdate) : IRequest<Checking>
    {
        public Checking CheckingToUpdate { get; } = checkingToUpdate;
    }

    public class UpdateCheckingCommandHandler(ICheckingRepository checkingRepository) : IRequestHandler<UpdateCheckingCommand, Checking>
    {
        public async Task<Checking> Handle(UpdateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await checkingRepository.UpdateAsync(request.CheckingToUpdate);
        }
    }
}
