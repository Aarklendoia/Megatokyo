using MediatR;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class SaveStripCommand : IRequest<Unit>
    {
    }

    public class SaveStripCommandHandler(IStripRepository stripRepository) : IRequestHandler<SaveStripCommand, Unit>
    {
        async Task<Unit> IRequestHandler<SaveStripCommand, Unit>.Handle(SaveStripCommand request, CancellationToken cancellationToken)
        {
            await stripRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
