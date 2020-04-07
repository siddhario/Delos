using System;
using System.Collections.Generic;
using System.Linq;
using Delos.Contexts;
using Delos.Model;
using Delos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebShopSyncController : ControllerBase
    {
        private DelosDbContext _dbContext;
        private readonly ILogger<KorisnikController> _logger;
        private IConfiguration _configuration;

        public WebShopSyncController(IConfiguration configuration, DelosDbContext context, ILogger<KorisnikController> logger)
        {
            _configuration = configuration;
            _dbContext = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult List()
        {
            var syncServices = _configuration.GetSection("Services").Get<List<SyncServiceConfig>>();
            return Ok(syncServices);
        }

        [HttpGet]
        [Route("sync")]
        public IActionResult Sync(int serviceId)
        {
            var artikli = new List<artikal>();
            var syncServices = _configuration.GetSection("Services").Get<List<SyncServiceConfig>>();
            var ss = syncServices.FirstOrDefault(ss => ss.Id == serviceId);

            if (ss != null)
            {
                string objectToInstantiate = ss.Implementation + ", Delos";
                var objectType = Type.GetType(objectToInstantiate);
                try
                {
                    var serviceInstance = Activator.CreateInstance(objectType) as ISyncService;
                    artikli = serviceInstance.Sync();
                    serviceInstance.UdpateDb(_dbContext, artikli);
                }
                catch(Exception ex)
                {

                }
            }
            return Ok(artikli);
        }

    }
}
