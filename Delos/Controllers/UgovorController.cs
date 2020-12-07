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
                    partner.telefon = ugovor.kupac_telefon;
                    partner.broj_lk = ugovor.kupac_broj_lk;
                    partner.maticni_broj = ugovor.kupac_maticni_broj;
                    partner.tip = "P";

                    ugovor.partner = partner;
                }
                else
                {
                    partner = _dbContext.partner.Where(p => p.sifra == ugovor.kupac_sifra).FirstOrDefault();

                    partner.naziv = ugovor.kupac_naziv;
                    partner.adresa = ugovor.kupac_adresa;
                    partner.telefon = ugovor.kupac_telefon;
                    partner.broj_lk = ugovor.kupac_broj_lk;
                    partner.maticni_broj = ugovor.kupac_maticni_broj;

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
        [Route("updateRate")]
        [HttpPut]
        public IActionResult UpdateUgovorRata(ugovor_rata rata)
        {
            var ugovorRata = _dbContext.ugovor_rata.FirstOrDefault(r => r.ugovorbroj == rata.ugovorbroj && r.broj_rate == rata.broj_rate);
            if (ugovorRata == null)
                return NotFound();
            else
            {
                try
                {
                    ugovorRata.uplaceno = rata.uplaceno;
                    ugovorRata.datum_placanja = rata.datum_placanja;
                    _dbContext.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [HttpGet]
        [Route("zakljuci")]
        public IActionResult ZakljuciUgovor(string broj)
        {
            var pon = _dbContext.ugovor.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    pon.status = "Z";
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
        [Route("otkljucaj")]
        public IActionResult OtkljucajUgovor(string broj)
        {
            var pon = _dbContext.ugovor.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    pon.status = "E";
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
        [Route("realizovan")]
        public IActionResult UgovorRealizovan (string broj)
        {
            var pon = _dbContext.ugovor.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    pon.status = "R";
                    _dbContext.SaveChanges();
                    return Ok(pon);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [HttpPut]
        public IActionResult UpdateUgovor(ugovor ugovor)
        {
            ugovor.Korisnik = null;
            var pon = _dbContext.ugovor.Include(u => u.rate).FirstOrDefault(p => p.broj == ugovor.broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    foreach (var r in pon.rate)
                        _dbContext.Remove(r);

                    _dbContext.SaveChanges();
                    Helper.CopyPropertiesTo<ugovor, ugovor>(ugovor, pon);

                    partner partner;
                    if (ugovor.partner.sifra == null)
                    {
                        partner = new partner();

                        partner.naziv = ugovor.kupac_naziv;
                        partner.adresa = ugovor.kupac_adresa;
                        partner.telefon = ugovor.kupac_telefon;
                        partner.broj_lk = ugovor.kupac_broj_lk;
                        partner.maticni_broj = ugovor.kupac_maticni_broj;
                        partner.tip = "F";
                        pon.kupac_sifra = null;
                        pon.partner = partner;
                    }
                    else
                    {
                        partner = _dbContext.partner.Where(p => p.sifra == ugovor.kupac_sifra).FirstOrDefault();

                        partner.naziv = ugovor.kupac_naziv;
                        partner.adresa = ugovor.kupac_adresa;
                        partner.telefon = ugovor.kupac_telefon;
                        partner.broj_lk = ugovor.kupac_broj_lk;
                        partner.maticni_broj = ugovor.kupac_maticni_broj;
                        partner.tip = "F";
                        _dbContext.SaveChanges();

                        pon.partner = partner;

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
            var pon = _dbContext.ugovor.Include(u=>u.rate).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                string file = Helper.StampaUgovora(pon);
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
