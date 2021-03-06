﻿using System;
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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PrijavaController : ControllerBase
    {

        private DelosDbContext _dbContext;

        private readonly ILogger<PrijavaController> _logger;
        public PrijavaController(DelosDbContext context, ILogger<PrijavaController> logger)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public QueryResult Get(int page, int pageSize, string searchText)
        {
           int resultCount = 0; 
            if (page == 1)
            {
                resultCount = _dbContext.prijava
                .Where(p => searchText == null || p.broj.Contains(searchText) || p.kupac_ime.ToLower().Contains(searchText.ToLower()) || p.predmet.ToLower().Contains(searchText.ToLower())
                     ).Count();
            }
            var result = _dbContext.prijava.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.dobavljac_partner).ToList()
                .Where(p => searchText == null || p.broj.Contains(searchText) || p.kupac_ime.ToLower().Contains(searchText.ToLower()) || p.predmet.ToLower().Contains(searchText.ToLower())).OrderByDescending(p => p.datum.Value.Year).ThenByDescending(p => p.broj)
                .Skip(pageSize * (page - 1)).Take(pageSize).ToList();

            return new QueryResult() { data = result, pageCount = (resultCount / pageSize) + 1, resultCount = resultCount };
        }

        //[HttpGet]
        //[Route("search")]
        //public IEnumerable<prijava> Search(string naziv)
        //{
        //    var prijave = _dbContext.prijava.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.dobavljac_partner).Where(p => naziv == null || p.kupac_ime.ToLower().Contains(naziv.ToLower()) || p.predmet.ToLower().Contains(naziv.ToLower())).OrderByDescending(p => p.datum.Value.Year).ThenByDescending(p => p.broj);
        //    return prijave;

        //}

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

        //[HttpGet]
        //[Route("radniNalog")]
        //public IActionResult radniNalog(string broj)
        //{
        //    var pon = _dbContext.prijava.FirstOrDefault(p => p.broj == broj);
        //    if (pon == null)
        //        return NotFound();
        //    else
        //    {
        //        string file = Helper.StampaRadnogNaloga(pon);
        //        var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        //        _logger.LogError(file);
        //        return new FileStreamResult(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        //    }
        //}



    }
}
