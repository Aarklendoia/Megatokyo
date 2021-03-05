using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Megatokyo.Server.Models;
using Megatokyo.Server.Database.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Megatokyo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public HomeController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet]
        public async Task<Home> GetHome()
        {
            Home home = new Home(_repoWrapper);
            await home.Load().ConfigureAwait(false);
            return home;
        }
    }
}