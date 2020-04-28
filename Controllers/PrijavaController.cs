using System;
using System.Collections.Generic;
using System.Linq;
using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PrijavaController : ControllerBase
    {

        private DelosDbContext _dbContext;

        public PrijavaController(DelosDbContext context, ILogger<PrijavaController> logger)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<prijava> Get()
        {

            var prijave = _dbContext.prijava.Include(p=>p.partner).Include(p=>p.Korisnik).Include(p => p.dobavljac_partner).ToList().OrderByDescending(p => p.broj);
            return prijave;

        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<prijava> Search(string naziv)
        {
            var prijave = _dbContext.prijava.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.dobavljac_partner).Where(p => naziv==null|| p.kupac_ime.ToLower().Contains(naziv.ToLower())|| p.predmet.ToLower().Contains(naziv.ToLower()));;
            return prijave;

        }

        [HttpDelete]
        [Route("obrisiPrijavu")]
        public IActionResult DeletePrijava(string broj)
        {
            var prijava = _dbContext.prijava.FirstOrDefault(p => p.broj == broj);
            if (prijava != null)
            {
                _dbContext.prijava.Remove(prijava);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }


        [HttpPost]
        public IActionResult InsertPrijava(prijava prijava)
        {
            try
            {
                var prijave = _dbContext.prijava.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.dobavljac_partner).Where(p => p.datum.Value.Year == DateTime.Now.Year);
                string maxPrijavaBroj = null;
                int year = prijava.datum.Value.Year;
                int? broj = null;
                if (prijave != null && prijave.Count() > 0)
                {
                    maxPrijavaBroj = prijave.Max(p => p.broj);
                    int dbroj = int.Parse(maxPrijavaBroj.Split("/")[0]);
                    broj = dbroj + 1;
                }
                else
                    broj = 1;
                prijava.serviser_primio = User.Identity.Name;
                prijava.broj = broj.Value.ToString("D5") + "/" + year.ToString();

                partner partner;
                if (prijava.partner.sifra == null)
                {
                    partner = new partner();

                    partner.naziv = prijava.kupac_ime;
                    partner.adresa = prijava.kupac_adresa;
                    partner.telefon = prijava.kupac_telefon;
                    partner.email = prijava.kupac_email;
                    partner.tip = "P";

                    prijava.partner = partner;
                }
                else
                {
                    partner = _dbContext.partner.Where(p => p.sifra == prijava.kupac_sifra).FirstOrDefault();

                    partner.naziv = prijava.kupac_ime;
                    partner.adresa = prijava.kupac_adresa;
                    partner.telefon = prijava.kupac_telefon;
                    partner.email = prijava.kupac_email;
                    partner.tip = "P";
                    _dbContext.SaveChanges();

                    prijava.partner = partner;
                }
                if (prijava.dobavljac_partner != null)
                {
                    partner = _dbContext.partner.Where(p => p.sifra == prijava.dobavljac_sifra).FirstOrDefault();
                    prijava.dobavljac_partner = partner;
                }

                _dbContext.prijava.Add(prijava);
                _dbContext.SaveChanges();

                prijava.Korisnik = _dbContext.korisnik.FirstOrDefault(k => k.korisnicko_ime == prijava.serviser_primio);
                return Ok(prijava);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult UpdatePartner(prijava prijava)
        {
            prijava.Korisnik = null;
            var pon = _dbContext.prijava.FirstOrDefault(p => p.broj == prijava.broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    Helper.CopyPropertiesTo<prijava, prijava>(prijava, pon);

                    partner partner;
                    if (prijava.partner.sifra == null)
                    {
                        partner = new partner();

                        partner.naziv = prijava.kupac_ime;
                        partner.adresa = prijava.kupac_adresa;
                        partner.telefon = prijava.kupac_telefon;
                        partner.email = prijava.kupac_email;
                        partner.tip = "P";
                        pon.kupac_sifra = null;
                        pon.partner = partner;
                    }
                    else
                    {
                        partner = _dbContext.partner.Where(p => p.sifra == prijava.kupac_sifra).FirstOrDefault();

                        partner.naziv = prijava.kupac_ime;
                        partner.adresa = prijava.kupac_adresa;
                        partner.telefon = prijava.kupac_telefon;
                        partner.email = prijava.kupac_email;
                        partner.tip = "P";
                        _dbContext.SaveChanges();

                        pon.partner = partner;

                    }
                    if (prijava.dobavljac_partner != null)
                    {
                        partner = _dbContext.partner.Where(p => p.sifra == prijava.dobavljac_sifra).FirstOrDefault();
                        prijava.dobavljac_partner = partner;
                    }
                    _dbContext.SaveChanges();
                    return Ok(pon);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

    }
}
