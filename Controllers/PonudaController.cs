﻿using System;
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

        private DelosDbContext _dbContext;

 
        private readonly ILogger<PonudaController> _logger;

        public PonudaController(DelosDbContext context, ILogger<PonudaController> logger)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<ponuda> Get()
        {

            var ponude = _dbContext.ponuda.Include(p=>p.stavke).Include(p=>p.partner).OrderByDescending(p=>p.broj).ToList();
            return ponude;        
        }



        [HttpGet]
        [Route("getbybroj")]
        public ponuda GetByBroj(string broj)
        {
            var ponuda = _dbContext.ponuda.Include(p => p.stavke).Include(p => p.partner).FirstOrDefault(p=>p.broj==broj);
            return ponuda;
        }

        [HttpPost]
        public IActionResult InsertPonuda(ponuda ponuda)
        {
            try
            {
                var ponude = _dbContext.ponuda.Where(p => p.datum.Year == DateTime.Now.Year);
                string maxPonudaBroj = null;
                int year = ponuda.datum.Year;
                int? broj = null;
                if (ponude != null && ponude.Count() > 0)
                {
                    maxPonudaBroj = ponude.Max(p => p.broj);
                    int dbroj = int.Parse(maxPonudaBroj.Split("/")[0]);
                    broj = dbroj + 1;
                }
                else
                    broj = 1;
                ponuda.radnik = "dario";
                ponuda.status = "E";
                //ponuda.partner_sifra = 14;
                ponuda.broj = broj.Value.ToString("D5") + "/" + year.ToString();
                ponuda.iznos_bez_rabata = 0;
                ponuda.iznos_sa_pdv = 0;
                ponuda.iznos_sa_pdv = 0;
                ponuda.pdv = 0;
                ponuda.iznos_sa_rabatom=0;
                ponuda.rabat = 0;
                partner partner;
                if (ponuda.partner.sifra == null)
                {
                    partner = new partner();

                    partner.naziv = ponuda.partner_naziv;
                    partner.adresa = ponuda.partner_adresa;
                    partner.maticni_broj = ponuda.partner_jib;
                    partner.telefon = ponuda.partner_telefon;
                    partner.email = ponuda.partner_email;
                    partner.tip = "P";

                    ponuda.partner = partner;
                }
                else
                {
                    partner = _dbContext.partner.Where(p => p.sifra == ponuda.partner_sifra).FirstOrDefault();

                    partner.naziv = ponuda.partner_naziv;
                    partner.adresa = ponuda.partner_adresa;
                    partner.maticni_broj = ponuda.partner_jib;
                    partner.telefon = ponuda.partner_telefon;
                    partner.email = ponuda.partner_email;
                    partner.tip = "P";
                    _dbContext.SaveChanges();

                    ponuda.partner = partner;                    
                }


                _dbContext.ponuda.Add(ponuda);
                _dbContext.SaveChanges();
                return Ok(ponuda);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult UpdatePonuda(ponuda ponuda)
        {
            //var ponude = _dbContext.ponuda.Where(p => p.datum.Year == DateTime.Now.Year);
            //string maxPonudaBroj = null;
            //int year = ponuda.datum.Year;
            //int? broj = null;
            //if (ponude != null && ponude.Count() > 0)
            //{
            //    maxPonudaBroj = ponude.Max(p => p.broj);
            //    int dbroj = int.Parse(maxPonudaBroj.Split("/")[0]);
            //    broj = dbroj + 1;
            //}
            //else
            //    broj = 0;

            //ponuda.broj = broj.Value.ToString("D5") + "/" + year.ToString();

            var pon = _dbContext.ponuda.FirstOrDefault(p =>p.broj== ponuda.broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    Helper.CopyPropertiesTo<ponuda, ponuda>(ponuda, pon);

                    partner partner;
                    if (ponuda.partner.sifra == null)
                    {
                        partner = new partner();

                        partner.naziv = ponuda.partner_naziv;
                        partner.adresa = ponuda.partner_adresa;
                        partner.maticni_broj = ponuda.partner_jib;
                        partner.telefon = ponuda.partner_telefon;
                        partner.email = ponuda.partner_email;
                        partner.tip = "P";
                        pon.partner_sifra = null;
                        //_dbContext.partner.Add(partner);
                        //_dbContext.SaveChanges();
                        pon.partner = partner;
                        //pon.partner_sifra = partner.sifra;
                    }
                    else
                    {
                        partner = _dbContext.partner.Where(p => p.sifra == ponuda.partner_sifra).FirstOrDefault();

                        partner.naziv = ponuda.partner_naziv;
                        partner.adresa = ponuda.partner_adresa;
                        partner.maticni_broj = ponuda.partner_jib;
                        partner.telefon = ponuda.partner_telefon;
                        partner.email = ponuda.partner_email;
                        partner.tip = "P";
                        _dbContext.SaveChanges();

                        pon.partner = partner;
                    }

                    _dbContext.SaveChanges();
                    return Ok();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [HttpPost]
        [Route("stavka_add")]
        public IActionResult InsertStavkaPonuda(ponuda_stavka stavka)
        {
            int? ponuda_stavka = null;
            var stavke = _dbContext.ponuda_stavka.Where(ps => ps.ponuda_broj == stavka.ponuda_broj);
            if(stavke!=null&&stavke.Count()>0)
                ponuda_stavka = stavke.Max(ps => ps.stavka_broj);

            stavka.stavka_broj = ponuda_stavka==null? 1:(ponuda_stavka.Value+1);
            _dbContext.Add(stavka);
            _dbContext.SaveChanges();
            return Ok();
        }
        [HttpPut]
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
