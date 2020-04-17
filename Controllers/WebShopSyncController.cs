using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Delos.Contexts;
using Delos.Model;
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
        [Route("artikli")]
        public IEnumerable<artikal> GetArtikli()
        {
            return _dbContext.artikal.ToList();
        }

        [HttpGet]
        [Route("kategorije")]
        public IEnumerable<kategorija> GetKategorije()
        {
            return _dbContext.kategorija.ToList();
        }
        [HttpGet]
        [Route("updateKategorije")]
        public IActionResult UpdateKategorije()
        {
            var artikli = _dbContext.artikal.ToList();
            foreach (var kat in _dbContext.kategorija)
            {
                if (kat.kategorije_dobavljaca != null)
                {
                    foreach (var kd in kat.kategorije_dobavljaca)
                    {
                        foreach (var art in artikli)
                        {
                            if (art.vrste != null)
                            {
                                foreach (var vrsta in art.vrste)
                                {
                                    if (vrsta == kd)
                                        art.kategorija = kat.naziv;
                                }
                            }
                        }
                    }
                }
            }
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("artikliSearch")]
        public IEnumerable<artikal> Search(string naziv, string kategorija, string dostupnost, string dobavljac, string loadAll)
        {

            if (loadAll == "0" && (naziv == null || naziv.Length < 3))
                return null;
            var exp = "";
            if (naziv != null)
            {
                var words = naziv.Split(" ");

                for (int i = 0; i < words.Length; i++)
                {
                    exp += "(?=.*" + words[i].ToLower() + ".*)";
                }
                exp += ".+";
            }


            return _dbContext.artikal.Where(it =>
                (
                    loadAll == "1" ||
                    Regex.IsMatch(it.naziv.ToLower(), exp)
                )
                &&
                (
                    kategorija == null ||
                    it.kategorija == kategorija
                )
                &&
                (
                    dobavljac == null ||
                    dobavljac == it.dobavljac
                )
                &&
                (
                    dostupnost == null || dostupnost == "0" ||
                    (it.dostupnost != null && it.dostupnost != "0")
                )
            ).ToList().OrderBy(aa => aa.naziv);

            //var artikli = _dbContext.artikal.Where(p => p.naziv.ToLower().Contains(naziv.ToLower()) && p.dostupnost!=null && p.dostupnost!="0");
            //return artikli.ToList();
        }



        [HttpGet]
        [Route("artikliGetBySifra")]
        public artikal SearchBySifra(string sifra)
        {
            return _dbContext.artikal.First(p => p.sifra == sifra);
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
        public async System.Threading.Tasks.Task<IActionResult> SyncAsync(int serviceId)
        {
            var artikli = new List<artikal>();
            var syncServices = _configuration.GetSection("Services").Get<List<SyncServiceConfig>>();
            var ss = syncServices.FirstOrDefault(ss => ss.Id == serviceId);

            if (ss != null)
            {
                string objectToInstantiate = ss.Implementation + ", Delos";
                var objectType = Type.GetType(objectToInstantiate);

                var serviceInstance = Activator.CreateInstance(objectType) as ISyncService;
                serviceInstance.Config.Description = ss.Description;
                artikli = await serviceInstance.SyncAsync();
                serviceInstance.UdpateDb(_dbContext, artikli);

            }
            return Ok(artikli);
        }

    }
}
