using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetChapterQuery : IRequest<ChapterDomain>
    {
        public int Number { get; set; }

        public GetChapterQuery(int number)
        {
            Number = number;
        }
    }

    public class GetChapterQueryHandler : IRequestHandler<GetChapterQuery, ChapterDomain>
    {
        private readonly IEntitiesRepository _entityRepository;

        public GetChapterQueryHandler(IEntitiesRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public async Task<ChapterDomain> Handle(GetChapterQuery request, CancellationToken cancellationToken)
        {
            return await _entityRepository.Chapters.GetAsync(request.Number);
        }
    }
}
