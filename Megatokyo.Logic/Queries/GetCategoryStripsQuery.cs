using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetCategoryStripsQuery : IRequest<IEnumerable<StripDomain>>
    {
        public string Category { get; set; }

        public GetCategoryStripsQuery(string category)
        {
            Category = category;
        }
    }

    public class GetCategoryStripsQueryHandler : IRequestHandler<GetCategoryStripsQuery, IEnumerable<StripDomain>>
    {
        private readonly IStripRepository _stripRepository;

        public GetCategoryStripsQueryHandler(IStripRepository entityRepository)
        {
            _stripRepository = entityRepository;
        }

        public async Task<IEnumerable<StripDomain>> Handle(GetCategoryStripsQuery request, CancellationToken cancellationToken)
        {
            return await _stripRepository.GetCategoryAsync(request.Category);
        }
    }
}
