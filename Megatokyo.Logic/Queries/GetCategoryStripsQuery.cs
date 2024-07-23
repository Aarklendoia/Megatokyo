using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetCategoryStripsQuery(string category) : IRequest<IEnumerable<Strip>>
    {
        public string Category { get; set; } = category;
    }

    public class GetCategoryStripsQueryHandler(IStripRepository entityRepository) : IRequestHandler<GetCategoryStripsQuery, IEnumerable<Strip>>
    {
        public async Task<IEnumerable<Strip>> Handle(GetCategoryStripsQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetCategoryAsync(request.Category);
        }
    }
}
