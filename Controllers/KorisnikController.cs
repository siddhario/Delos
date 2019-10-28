using System.Collections.Generic;
using System.Linq;
using Delos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KorisnikController : ControllerBase
    {

        private DelosDbContext _dbContext;

 
        private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(DelosDbContext context, ILogger<KorisnikController> logger)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<korisnik> Get()
        {

            var korisnici = _dbContext.korisnik.ToList();
            return korisnici;
        
        }
    }
}
