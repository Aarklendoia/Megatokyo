using Megatokyo.Server.Database.Contracts;
using Megatokyo.Server.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Megatokyo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewRantsController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public NewRantsController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet("{date}")]
        public async Task<IEnumerable<Rants>> GetNewRants([FromRoute] DateTime date)
        {
            // L'URL doit être au format /api/newrants/2018-07-30T23:08:00
            return await _repoWrapper.Rants.FindByConditionAsync(s => s.Date >= date.Date).ConfigureAwait(false);
        }
    }
}