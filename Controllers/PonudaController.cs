using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PonudaController : ControllerBase
    {

        private BloggingContext _dbContext;

 
        private readonly ILogger<PonudaController> _logger;

        public PonudaController(BloggingContext context, ILogger<PonudaController> logger)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<ponuda> Get()
        {

            var ponude = _dbContext.ponuda.Include(p=>p.stavke).ToList();
            return ponude;
        
        }

        [HttpGet]
        [Route("getbybroj")]
        public ponuda GetByBroj(string broj)
        {
            var ponuda = _dbContext.ponuda.Include(p => p.stavke).FirstOrDefault(p=>p.broj==broj);
            return ponuda;

        }

        [HttpPost]
        [Route("stavka_add")]
        public IActionResult InsertStavkaPonuda(ponuda_stavka stavka)
        {
            int? ponuda_stavka = _dbContext.ponuda_stavka.Where(ps => ps.ponuda_broj == stavka.ponuda_broj).Max(ps => ps.stavka_broj);

            stavka.stavka_broj = ponuda_stavka==null? 1:(ponuda_stavka.Value+1);
            _dbContext.Add(stavka);
            _dbContext.SaveChanges();
            return Ok();
        }
        [HttpPost]
        [Route("stavka_update")]
        public IActionResult UpdateStavkaPonuda(ponuda_stavka stavka)
        {
            var ponuda_stavka = _dbContext.ponuda_stavka.Where(ps => ps.ponuda_broj == stavka.ponuda_broj && ps.stavka_broj == stavka.stavka_broj).FirstOrDefault();
            if (ponuda_stavka == null)
                return NotFound();

            Helper.CopyPropertiesTo<ponuda_stavka, ponuda_stavka>(stavka, ponuda_stavka);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("stavka_delete")]
        public IActionResult DeleteStavkaPonuda(string ponuda_broj, int stavka_broj)
        {
            var ponuda_stavka = _dbContext.ponuda_stavka.Where(ps => ps.ponuda_broj == ponuda_broj && ps.stavka_broj == stavka_broj).FirstOrDefault();
            if (ponuda_stavka == null)
                return NotFound();

            _dbContext.Remove(ponuda_stavka);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("/ponuda_stavka")]
        public IEnumerable<ponuda_stavka> GetStavke(string ponuda_broj)
        {
            var stavke = _dbContext.ponuda_stavka.Where(sp=> sp.ponuda_broj==ponuda_broj).ToList();
            return stavke;

        }
    }
}
