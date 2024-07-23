using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Commands
{
    public class CreateStripCommand(Strip stripToCreate) : IRequest<Strip>
    {
        public Strip StripToCreate { get; } = stripToCreate;
    }

    public class CreateStripCommandHandler(IStripRepository stripRepository) : IRequestHandler<CreateStripCommand, Strip>
    {
        public async Task<Strip> Handle(CreateStripCommand request, CancellationToken cancellationToken)
        {
            return await stripRepository.CreateAsync(request.StripToCreate);
        }
    }
}
