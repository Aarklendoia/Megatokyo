using Megatokyo.Client.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Client.Core.Services
{
    internal interface IRantsService
    {
        Task<IEnumerable<Rant>> GetAllRants();
        Task<Rant> GetRantById(int id);

    }
}
