using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Helpers;
using Shared.Model;

namespace WebApplication3.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UgovorController : ControllerBase
    {

        private DelosDbContext _dbContext;
        public IConfiguration _con;
        private readonly ILogger<UgovorController> _logger;
        public UgovorController(DelosDbContext context, ILogger<UgovorController> logger, IConfiguration con)
        {
            _logger = logger;
            _con = con;
            _dbContext = context;
        }

        [HttpGet]
        public QueryResult Get(int page, int pageSize, string searchText)
        {
            int resultCount = 0;
            if (page == 1)
            {
                resultCount = _dbContext.ugovor.Where(p => searchText == null || p.broj.Contains(searchText) || p.kupac_naziv.ToLower().Contains(searchText.ToLower())).Count();
            }
            var result = _dbContext.ugovor.Where(p => searchText == null || p.broj.Contains(searchText) || p.kupac_naziv.ToLower().Contains(searchText.ToLower())).Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.rate)
               .OrderByDescending(p => p.datum.Year).ThenByDescending(p => p.broj)
                .Skip(pageSize * (page - 1)).Take(pageSize).ToList();

            return new QueryResult() { data = result, pageCount = (resultCount / pageSize) + 1, resultCount = resultCount };
        }

        //[HttpGet]
        //[Route("search")]
        //public IEnumerable<ugovor> Search(string naziv)
        //{
        //    var ugovori = _dbContext.ugovor.Include(p => p.partner).Include(p => p.Korisnik).Where(p => naziv == null || p.kupac_naziv.ToLower().Contains(naziv.ToLower()));
        //    return ugovori;

        //}

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
                ugovor.preostalo_za_uplatu = ugovor.iznos_sa_pdv - ugovor.inicijalno_placeno;
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
        public IActionResult UgovorRealizovan(string broj)
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
            var pon = _dbContext.ugovor.Include(u => u.rate).FirstOrDefault(p => p.broj == broj);
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
        [Route("potvrda")]
        public IActionResult Potvrda(string broj, int broj_rate)
        {
            var pon = _dbContext.ugovor.Include(u => u.rate).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                string file = Helper.StampaPotvrdeOPlacanju(pon, broj_rate);
                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                _logger.LogError(file);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel");
            }
        }

        [HttpGet]
        [Route("pregledUplata")]
        public async Task<IActionResult> PregledUplata(DateTime datumOd, DateTime datumDo)
        {
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            List<string> sume = new List<string>();
            string rpt = "";
            string title = "";
            string report = "Pregled uplata u periodu";
            rpt = "rptPregledUplata";
            parameters.Add(new NpgsqlParameter() { ParameterName = "datefrom", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date, Value = datumOd.Date });
            parameters.Add(new NpgsqlParameter() { ParameterName = "dateto", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date, Value = datumDo.Date });
            //}
            title = report + " " + datumOd.ToString("dd.MM.yyyy") + "-" + datumDo.ToString("dd.MM.yyyy");
            sume.Add("uplaćeno");
            var ds = await ReportManager.ExecuteProcedureReport(rpt, parameters, _con.GetConnectionString("Delos"));

            var reportFile = ReportBuilder.BuildReport(report, title, ds, sume);
            if (reportFile != null)
            {
                var fileStream = new FileStream(reportFile, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel");
            }
            else
                return BadRequest();
        }
        [HttpGet]
        [Route("pregledDugovanja")]
        public async Task<IActionResult> PregledDugovanja(DateTime datumOd, DateTime datumDo)
        {
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            List<string> sume = new List<string>();
            string rpt = "";
            string title = "";
            rpt = "rptPregledDugovanja";
            string report = "Pregled dugovanja u periodu";


            parameters.Add(new NpgsqlParameter() { ParameterName = "datefrom", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date, Value = datumOd.Date });
            parameters.Add(new NpgsqlParameter() { ParameterName = "dateto", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date, Value = datumDo.Date });

            title = report + " " + datumOd.ToString("dd.MM.yyyy") + "-" + datumDo.ToString("dd.MM.yyyy");
            sume.Add("dug");
            var ds = await ReportManager.ExecuteProcedureReport(rpt, parameters, _con.GetConnectionString("Delos"));

            var reportFile = ReportBuilder.BuildReport(report, title, ds, sume);
            if (reportFile != null)
            {
                var fileStream = new FileStream(reportFile, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel");
            }
            else
                return BadRequest();
        }

    }
}
