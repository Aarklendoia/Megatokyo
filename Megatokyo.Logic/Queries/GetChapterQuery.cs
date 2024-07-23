using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;

namespace Megatokyo.Logic.Queries
{
    public class GetChapterQuery(string category) : IRequest<Chapter>
    {
        public string Category { get; set; } = category;
    }

    public class GetChapterQueryHandler(IChapterRepository entityRepository) : IRequestHandler<GetChapterQuery, Chapter>
    {
        public async Task<Chapter> Handle(GetChapterQuery request, CancellationToken cancellationToken)
        {
            return await entityRepository.GetAsync(request.Category);
        }
    }
}
