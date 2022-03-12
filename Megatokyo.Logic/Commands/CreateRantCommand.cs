using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class CreateRantCommand : IRequest<Rant>
    {
        public Rant RantToCreate { get; }

        public CreateRantCommand(Rant rantToCreate)
        {
            RantToCreate = rantToCreate;
        }
    }

    public class CreateRantCommandHandler : IRequestHandler<CreateRantCommand, Rant>
    {
        private readonly IRantRepository _rantRepository;

        public CreateRantCommandHandler(IRantRepository rantRepository)
        {
            _rantRepository = rantRepository;
        }

        public async Task<Rant> Handle(CreateRantCommand request, CancellationToken cancellationToken)
        {
            return await _rantRepository.CreateAsync(request.RantToCreate);
        }
    }
}
