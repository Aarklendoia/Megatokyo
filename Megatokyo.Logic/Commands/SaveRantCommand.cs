using MediatR;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class SaveRantCommand : IRequest<Unit>
    {
    }

    public class SaveRantCommandHandler(IRantRepository rantRepository) : IRequestHandler<SaveRantCommand, Unit>
    {
        async Task<Unit> IRequestHandler<SaveRantCommand, Unit>.Handle(SaveRantCommand request, CancellationToken cancellationToken)
        {
            await rantRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
