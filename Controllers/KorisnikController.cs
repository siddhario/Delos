using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KorisnikController : ControllerBase
    {

        private BloggingContext _dbContext;

 
        private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(BloggingContext context, ILogger<KorisnikController> logger)
        {
            _logger = logger;
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
