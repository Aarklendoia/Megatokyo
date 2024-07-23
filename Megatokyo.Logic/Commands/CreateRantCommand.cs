using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class CreateRantCommand(Rant rantToCreate) : IRequest<Rant>
    {
        public Rant RantToCreate { get; } = rantToCreate;
    }

    public class CreateRantCommandHandler(IRantRepository rantRepository) : IRequestHandler<CreateRantCommand, Rant>
    {
        public async Task<Rant> Handle(CreateRantCommand request, CancellationToken cancellationToken)
        {
            return await rantRepository.CreateAsync(request.RantToCreate);
        }
    }
}
