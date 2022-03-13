using Megatokyo.Client.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Client.Core.Services
{
    public interface IRantsApiClient
    {
        Task<IEnumerable<Rant>> GetAllRants(/*CancellationToken cancellationToken*/);
        Task<Rant> GetRantById(int id/*, CancellationToken cancellationToken*/);

    }
}
