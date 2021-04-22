using MediatR;
using Megatokyo.Domain;
using Megatokyo.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Megatokyo.Logic.Queries
{
    public class GetCheckingQuery : IRequest<CheckingDomain>
    {
        public int Number { get; set; }

        public GetCheckingQuery(int number)
        {
            Number = number;
        }
    }

    public class GetCheckingQueryHandler : IRequestHandler<GetCheckingQuery, CheckingDomain>
    {
        private readonly ICheckingRepository _checkingRepository;

        public GetCheckingQueryHandler(ICheckingRepository entityRepository)
        {
            _checkingRepository = entityRepository;
        }

        public async Task<CheckingDomain> Handle(GetCheckingQuery request, CancellationToken cancellationToken)
        {
            return await _checkingRepository.GetAsync(request.Number);
        }
    }
}
