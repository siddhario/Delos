using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Model;

namespace WebApplication3.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UgovorController : ControllerBase
    {

        private DelosDbContext _dbContext;

        private readonly ILogger<UgovorController> _logger;
        public UgovorController(DelosDbContext context, ILogger<UgovorController> logger)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<ugovor> Get()
        {
            var ugovori = _dbContext.ugovor.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.rate).ToList().OrderByDescending(p => p.broj);
            return ugovori;

        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<ugovor> Search(string naziv)
        {
            var ugovori = _dbContext.ugovor.Include(p => p.partner).Include(p => p.Korisnik).Where(p => naziv == null || p.kupac_naziv.ToLower().Contains(naziv.ToLower()));
            return ugovori;

        }

        [HttpDelete]
        [Route("obrisiUgovor")]
        public IActionResult DeleteUgovor(string broj)
        {
            var ugovor = _dbContext.ugovor.FirstOrDefault(p => p.broj == broj);
            if (ugovor != null)
            {
                _dbContext.ugovor.Remove(ugovor);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }

    

        [HttpPost]
        public IActionResult InsertUgovor(ugovor ugovor)
        {
            try
            {
                var ugovori = _dbContext.ugovor.Include(p => p.partner).Include(p => p.Korisnik).Where(p => p.datum.Year == DateTime.Now.Year);
                string maxBroj = null;
                int year = ugovor.datum.Year;
                int? broj = null;
                if (ugovori != null && ugovori.Count() > 0)
                {
                    maxBroj = ugovori.Max(p => p.broj);
                    int dbroj = int.Parse(maxBroj.Split("/")[0]);
                    broj = dbroj + 1;
                }
                else
                    broj = 1;
                ugovor.radnik = User.Identity.Name;
                ugovor.broj = broj.Value.ToString("D5") + "/" + year.ToString();
                ugovor.uplaceno_po_ratama = 0;

                partner partner;
                if (ugovor.partner.sifra == null)
                {
                    partner = new partner();

                    partner.naziv = ugovor.kupac_naziv;
                    partner.adresa = ugovor.kupac_adresa;
                    partner.broj_lk = ugovor.kupac_broj_lk;
                    partner.tip = "P";

                    ugovor.partner = partner;
                }
                else
                {
                    partner = _dbContext.partner.Where(p => p.sifra == ugovor.kupac_sifra).FirstOrDefault();

                    partner.naziv = ugovor.kupac_naziv;
                    partner.adresa = ugovor.kupac_adresa;
                    partner.broj_lk = ugovor.kupac_broj_lk;

                    partner.tip = "P";
                    _dbContext.SaveChanges();

                    ugovor.partner = partner;
                }

                ugovor.status = "E";
                _dbContext.ugovor.Add(ugovor);
                _dbContext.SaveChanges();

                ugovor.Korisnik = _dbContext.korisnik.FirstOrDefault(k => k.korisnicko_ime == ugovor.radnik);
                return Ok(ugovor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult UpdatePrijava(prijava prijava)
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

        [HttpGet]
        [Route("excel")]
        public IActionResult Excel(string broj)
        {
            var pon = _dbContext.prijava.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                string file = Helper.StampaPrijave(pon);
                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                _logger.LogError(file);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel");
            }
        }

        [HttpGet]
        [Route("excelRadniNalog")]
        public IActionResult ExcelRadniNalog(string broj)
        {
            var pon = _dbContext.prijava.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                string file = Helper.StampaRadnogNalogaExcel(pon);
                pon.broj_naloga = broj;
                _dbContext.SaveChanges();
                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                _logger.LogError(file);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel");
            }
        }
    }
}
