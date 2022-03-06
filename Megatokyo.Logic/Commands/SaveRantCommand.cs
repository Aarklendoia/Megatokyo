using MediatR;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class SaveRantCommand : IRequest<Unit>
    {
        public SaveRantCommand()
        {
        }
    }

    public class SaveRantCommandHandler : IRequestHandler<SaveRantCommand, Unit>
    {
        private readonly IRantRepository _rantRepository;

        public SaveRantCommandHandler(IRantRepository rantRepository)
        {
            _rantRepository = rantRepository;
        }

        async Task<Unit> IRequestHandler<SaveRantCommand, Unit>.Handle(SaveRantCommand request, CancellationToken cancellationToken)
        {
            await _rantRepository.SaveAsync();
            return Unit.Value;
        }
    }
}
