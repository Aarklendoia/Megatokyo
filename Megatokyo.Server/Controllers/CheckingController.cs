using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckingController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public CheckingController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet]
        public async Task<Checking> GetChecking()
        {
            IEnumerable<Checking> checkings = await _repoWrapper.Checking.FindByConditionAsync(c => c.ChekingId == 1);
            Checking checking = checkings.First();
            return checking;
        }
    }
}