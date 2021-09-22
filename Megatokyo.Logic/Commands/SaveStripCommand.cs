using MediatR;
using Megatokyo.Logic.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Commands
{
    public class SaveStripCommand : IRequest<Unit>
    {
        public SaveStripCommand()
        {
        }
    }

    public class SaveStripCommandHandler : IRequestHandler<SaveStripCommand, Unit>
    {
        private readonly IStripRepository _stripRepository;

        public SaveStripCommandHandler(IStripRepository stripRepository)
        {
            _stripRepository = stripRepository;
        }

        async Task<Unit> IRequestHandler<SaveStripCommand, Unit>.Handle(SaveStripCommand request, CancellationToken cancellationToken)
        {
            await _stripRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
