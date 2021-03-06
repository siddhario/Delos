﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Model;

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

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(string sifra)
        {
            var art = _dbContext.artikal.FirstOrDefault(a => a.sifra == sifra);
            if (art != null)
            {
                _dbContext.artikal.Remove(art);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("artikliImport")]
        public IActionResult artikliImport(List<artikal> artikli)
        {
            foreach (var art in artikli)
            {
                art.sifra = art.dobavljac.Substring(1, 3) + "_" + art.dobavljac_sifra;

                var artikal = _dbContext.artikal.FirstOrDefault(a => a.sifra == art.sifra);
                if (artikal == null)
                {
                    art.dobavljac = art.dobavljac.Substring(5);
                    art.aktivan = true;
                    art.dostupnost = art.kolicina.ToString();
                    art.kalkulacija = true;
                    art.zadnje_ucitavanje = DateTime.Now;
                    _dbContext.artikal.Add(art);
                }
                else
                {
                    artikal.barkod = art.barkod;
                    artikal.brend = art.brend;
                    artikal.cijena_mp = art.cijena_mp;
                    artikal.cijena_prodajna = art.cijena_prodajna;
                    artikal.cijena_sa_rabatom = art.cijena_sa_rabatom;
                    artikal.dostupnost = art.dostupnost;
                    artikal.kategorija = art.kategorija;
                    artikal.kolicina = art.kolicina;
                    artikal.dostupnost = art.kolicina.ToString();
                    artikal.naziv = art.naziv;
                    artikal.opis = art.opis;
                    artikal.zadnje_ucitavanje = DateTime.Now;
                }
            }
            _dbContext.SaveChanges();
            return Ok();
        }


        [HttpDelete]
        [Route("deleteKategorija")]
        public IActionResult DeleteKategorija(string sifra)
        {
            try
            {
                var kat = _dbContext.kategorija.FirstOrDefault(k => k.sifra == sifra);
                if (kat == null)
                    return NotFound();
                _dbContext.Remove(kat);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Helper.LogException(ex);
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("insertKategorija")]
        public IActionResult InsertKategorija(kategorija kategorija)
        {
            try
            {
                string maxsifra = null;

                int? sifra = null;
                if (_dbContext.kategorija != null && _dbContext.kategorija.Count() > 0)
                {
                    maxsifra = _dbContext.kategorija.Max(p => p.sifra);
                    int dbroj = int.Parse(maxsifra);
                    sifra = dbroj + 1;
                }
                else
                    sifra = 1;

                kategorija.sifra = sifra.ToString().PadLeft(3, '0');

                _dbContext.kategorija.Add(kategorija);
                _dbContext.SaveChanges();
                return Ok(kategorija);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("kategorije")]
        public IEnumerable<kategorija> GetKategorije()
        {
            return _dbContext.kategorija.OrderBy(k => k.naziv).ToList();
        }
        [HttpGet]
        [Route("updateKategorije")]
        public IActionResult UpdateKategorije()
        {
            Delos.Helpers.Helper.UpdateKategorije(_dbContext);
            return Ok();
        }

        [HttpPut]
        [Route("updateKategorija")]
        public IActionResult UpdateKategorije(kategorija kategorija)
        {
            var kat = _dbContext.kategorija.FirstOrDefault(k => k.sifra == kategorija.sifra);
            if (kat != null)
            {
                kat.marza = kategorija.marza;
                kat.aktivna = kategorija.aktivna;
                kat.kategorije_dobavljaca = kategorija.kategorije_dobavljaca;
                Helper.UpdateCijenaIKategorijaArtikala(_dbContext, kategorija);



                _dbContext.SaveChanges();
            }
            return Ok();
        }

        [HttpPut]
        [Route("updateArtikal")]
        public IActionResult UpdateArtikal(artikal artikal)
        {
            var art = _dbContext.artikal.FirstOrDefault(p => p.sifra == artikal.sifra);
            if (art != null)
            {
                art.aktivan = artikal.aktivan;
                _dbContext.SaveChanges();
                return Ok(art);
            }
            else
                return NotFound();

        }
        [HttpGet]
        [Route("artikliSearch")]
        public QueryResult Search(string naziv, string kategorija, string dostupnost, string dobavljac, string loadAll, string brend, string aktivan, int page, int pageSize)
        {

            if (loadAll == "0" && (naziv == null || naziv.Length < 3))
                return new QueryResult() { pageCount = 0, resultCount = 0 };
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
            int resultCount = 0;
            if (page == 1)
            {
                resultCount = _dbContext.artikal.Where(it =>
                    (
                        loadAll == "1" ||
                        (Regex.IsMatch(it.naziv.ToLower(), exp) || (it.sifra.Contains(naziv)))
                    )
                    &&
                    (
                        kategorija == null ||
                        it.kategorija == kategorija || (kategorija == "NULL" && it.kategorija == null)
                    )
                       &&
                    (
                        brend == null ||
                        it.brend == brend || (brend == "NULL" && it.brend == null)
                    )
                         &&
                    (
                        aktivan == "0" || aktivan == null ||
                        (it.aktivan == true && aktivan == "1") || (it.aktivan == false && aktivan == "2")
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
                ).Count();
            }
            var result = _dbContext.artikal.Include(a => a.istorija_cijena).Where(it =>
                      (
                          loadAll == "1" ||
                          (Regex.IsMatch(it.naziv.ToLower(), exp) || (it.sifra.Contains(naziv)))
                      )
                      &&
                      (
                          kategorija == null ||
                          it.kategorija == kategorija || (kategorija == "NULL" && it.kategorija == null)
                      )
                         &&
                      (
                          brend == null ||
                          it.brend == brend || (brend == "NULL" && it.brend == null)
                      )
                           &&
                      (
                          aktivan == "0" || aktivan == null ||
                          (it.aktivan == true && aktivan == "1") || (it.aktivan == false && aktivan == "2")
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
                );
            if (pageSize == 0)
                return new QueryResult() { data = result.OrderBy(aa => aa.naziv), pageCount = 1, resultCount = resultCount };
            else
                return new QueryResult() { data = result.OrderBy(aa => aa.naziv).Skip(pageSize * (page - 1)).Take(pageSize), pageCount = (resultCount / pageSize) + 1, resultCount = resultCount };

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
            var syncServices = _configuration.GetSection("Services").Get<List<ImportServiceConfig>>();
            return Ok(syncServices);
        }

        [HttpGet]
        [Route("import")]
        public IActionResult Import()
        {
            List<KeyValuePair<string, string>> lista = new List<KeyValuePair<string, string>>();
            lista.Add(new KeyValuePair<string, string>("ASBIS", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\Artikli_export_ASBIS.xlsx"));
            lista.Add(new KeyValuePair<string, string>("KIMTEC", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\Artikli_export_KIMTEC.xlsx"));
            lista.Add(new KeyValuePair<string, string>("COMTRADE", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\Copy of Artikli_export_ct kategorisano.xlsx"));
            lista.Add(new KeyValuePair<string, string>("UNIEXPERT", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\Artikli_export_UNIEXPERT  ZAVRSENO.xlsx"));
            lista.Add(new KeyValuePair<string, string>("MINT", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\mint veze2.xlsx"));
            lista.Add(new KeyValuePair<string, string>("AVTERA", @"C:\Users\Dario\source\repos\Delos\Delos\TestData\Artikli_export_AVTERA- ZAVRSENO.xlsx"));

            foreach (var file in lista)
            {
                var workbook = new XLWorkbook(file.Value);
                var ws1 = workbook.Worksheet("Veze");
                for (int i = 1; i < ws1.RowCount(); i++)
                {
                    var k1 = ws1.Cell(i + 1, 1).Value.ToString().Trim();

                    var k2 = ws1.Cell(i + 1, 2).Value.ToString().Trim();
                    if (k1 != "" && k2 != "")
                    {
                        var kat1 = _dbContext.kategorija.FirstOrDefault(k => k.naziv.ToLower() == k2.ToLower());
                        if (kat1 == null)
                        {
                            kat1 = new kategorija() { naziv = k2 };
                            string maxSifra = _dbContext.kategorija.Max(k => k.sifra);
                            if (maxSifra == null)
                                maxSifra = "0";
                            int maxSifraInt = int.Parse(maxSifra);
                            string novaSifra = (maxSifraInt + 1).ToString().PadLeft(3, '0');
                            kat1.sifra = novaSifra;
                            _dbContext.Add(kat1);
                        }
                        if (kat1.kategorije_dobavljaca == null)
                            kat1.kategorije_dobavljaca = new List<string>();



                        string katd = k1.Trim();
                        if (katd.EndsWith(";"))
                            katd = katd.Substring(0, katd.Length - 1).Trim();
                        var exist = kat1.kategorije_dobavljaca.FirstOrDefault(k => k.ToLower() == ("[" + file.Key + "] " + katd).ToLower());
                        if (exist == null)
                            kat1.kategorije_dobavljaca.Add("[" + file.Key + "] " + katd);

                        //foreach (var kd in k1.Split(";"))
                        //{
                        //    if (kd != "" && kd != ";")
                        //    {
                        //        string katd = kd.Trim();
                        //        var exist = kat1.kategorije_dobavljaca.FirstOrDefault(k => k.ToLower() == ("[" + file.Key + "] " + katd).ToLower());
                        //        if (exist == null)
                        //            kat1.kategorije_dobavljaca.Add("[" + file.Key + "] " + katd);
                        //    }
                        //}

                        _dbContext.SaveChanges();
                    }
                }
            }
            return Ok();
        }

        [HttpDelete]
        [Route("deletePhotoURL")]
        public IActionResult DeletePhotoURL(string sifraArtikla, string url)
        {
            var art = _dbContext.artikal.FirstOrDefault(a => a.sifra == sifraArtikla);
            if (art != null)
            {
                var existing = art.slike != null ? art.slike.FirstOrDefault(s => s == url) : null;
                if (existing == null)
                    return BadRequest();
                else
                {
                    art.slike.Remove(existing);
                    _dbContext.SaveChanges();
                    return Ok();
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("addPhotoURL")]
        public IActionResult AddPhotoURL(string sifraArtikla, string url)
        {
            var art = _dbContext.artikal.FirstOrDefault(a => a.sifra == sifraArtikla);
            if (art != null)
            {
                var existing = art.slike!=null?art.slike.FirstOrDefault(s => s == url):null;
                if (existing != null)
                    return BadRequest();
                else
                {
                    if (art.slike == null)
                        art.slike = new List<string>();
                    art.slike.Add(url);
                    _dbContext.SaveChanges();
                    return Ok();
                }
            }
            else
            {
                return NotFound();
            }
        }

        //[HttpGet]
        //[Route("sync")]
        //public async System.Threading.Tasks.Task<IActionResult> SyncAsync(int serviceId)
        //{
        //    var artikli = new List<artikal>();
        //    var syncServices = _configuration.GetSection("Services").Get<List<SyncServiceConfig>>();
        //    var ss = syncServices.FirstOrDefault(ss => ss.Id == serviceId);

        //    if (ss != null)
        //    {
        //        string objectToInstantiate = ss.Implementation + ", Delos";
        //        var objectType = Type.GetType(objectToInstantiate);

        //        var serviceInstance = Activator.CreateInstance(objectType) as ISyncService;
        //        serviceInstance.Config.Description = ss.Description;
        //        artikli = await serviceInstance.SyncAsync();
        //        serviceInstance.UdpateDb(_dbContext, artikli);

        //    }
        //    return Ok(artikli);
        //}

    }
}
