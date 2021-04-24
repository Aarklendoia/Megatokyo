using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class UpdateCheckingCommand : IRequest<CheckingDomain>
    {
        public CheckingDomain CheckingToUpdate { get; }

        public UpdateCheckingCommand(CheckingDomain checkingToUpdate)
        {
            CheckingToUpdate = checkingToUpdate;
        }
    }

    public class UpdateCheckingCommandHandler : IRequestHandler<UpdateCheckingCommand, CheckingDomain>
    {
        private readonly ICheckingRepository _checkingRepository;

        public UpdateCheckingCommandHandler(ICheckingRepository checkingRepository)
        {
            _checkingRepository = checkingRepository;
        }

        public async Task<CheckingDomain> Handle(UpdateCheckingCommand request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.UpdateAsync(request.CheckingToUpdate);
        }
    }
}
