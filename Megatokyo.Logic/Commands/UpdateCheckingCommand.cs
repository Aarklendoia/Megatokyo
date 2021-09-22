using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class UpdateCheckingCommand : IRequest<Checking>
    {
        public Checking CheckingToUpdate { get; }

        public UpdateCheckingCommand(Checking checkingToUpdate)
        {
            CheckingToUpdate = checkingToUpdate;
        }
    }

    public class UpdateCheckingCommandHandler : IRequestHandler<UpdateCheckingCommand, Checking>
    {
        private readonly ICheckingRepository _checkingRepository;

        public UpdateCheckingCommandHandler(ICheckingRepository checkingRepository)
        {
            _checkingRepository = checkingRepository;
        }

        public async Task<Checking> Handle(UpdateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.UpdateAsync(request.CheckingToUpdate);
        }
    }
}
