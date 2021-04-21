using MediatR;
using Megatokyo.Domain;

namespace Megatokyo.Logic.Commands
{
    public class CreateStripCommand : IRequest<StripDomain>
    {
        public StripDomain StripToCreate { get; }

        public CreateStripCommand(StripDomain stripToCreate)
        {
            StripToCreate = stripToCreate;
        }
    }
}
