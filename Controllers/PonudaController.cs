using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Delos;
using Delos.Klase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PonudaController : ControllerBase
    {

        private DelosDbContext _dbContext;
        private IConfiguration _configuration;


        private readonly ILogger<PonudaController> _logger;

        public PonudaController(DelosDbContext context, ILogger<PonudaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = context;
            _configuration = configuration;
        }
        //[AllowAnonymous]
        [HttpPost]
        [Route("uploadPDF")]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile blob, string broj)
        {
            string filePath = null;
            try
            {

                filePath = Path.Combine(_configuration["ContentPath"],
                   blob.FileName.Split(".")[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + blob.FileName.Split(".")[1]);

                var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
                if (pon == null)
                    return NotFound();
                else
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await blob.CopyToAsync(fileStream);
                    }



                    var mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(pon.Korisnik.email);
                    mailMessage.To.Add(pon.partner_email);
                    mailMessage.Subject = "Ponuda za " + pon.predmet;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = "<span style='font-size: 12pt; color: black;'>Poštovani ,<br/> u prilogu se nalazi ponuda. <br/><br/> Pozdrav</span>";


                    mailMessage.Attachments.Add(new Attachment(filePath));

                    var eml = filePath + ".eml";

                    mailMessage.Save(eml);
                    var fs = new FileStream(eml, FileMode.Open, FileAccess.Read);

                    return new FileStreamResult(fs, "message/rfc822");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, filePath);
                return BadRequest();
            }
        }



        [HttpGet]
        [Route("excel")]
        public IActionResult Excel(string broj)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                string file = Helper.Stampa(pon);
                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                _logger.LogError(file);
                return new FileStreamResult(fileStream, "application/vnd.ms-excel.sheet.macroEnabled.12");
            }
        }

        [HttpGet]
        [Route("email")]
        public IActionResult Email(string broj)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {

                //string dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "temp");
                string dir = "C:\\temp";

                string br = broj;
                string[] parts = broj.Split('/');
                string rb = parts[0];
                string year = parts[1];
                int rrb = int.Parse(rb);
                broj = rrb.ToString("D4") + "/" + year;
                string fileName = dir + "\\MINTICT_Ponuda_" + broj.Replace("/", "-") + ".pdf";


                string file = Helper.Stampa(pon);
                Process p = Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
                Thread.Sleep(4000);
                p.CloseMainWindow();

                var mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(pon.Korisnik.email);
                mailMessage.To.Add(pon.partner_email);
                mailMessage.Subject = "Ponuda za " + pon.predmet;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = "<span style='font-size: 12pt; color: black;'>Poštovani ,<br/> u prilogu se nalazi ponuda. <br/><br/> Pozdrav</span>";


                mailMessage.Attachments.Add(new Attachment(fileName));

                var eml = fileName + ".eml";

                mailMessage.Save(eml);
                var fileStream = new FileStream(eml, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, "message/rfc822");
            }
        }

        [HttpPut]
        [Route("zakljuciPonudu")]
        public IActionResult ZakljuciPonudu(string broj)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    var user = _dbContext.korisnik.FirstOrDefault(s => s.korisnicko_ime == User.Identity.Name);
                    if (user.admin == true || pon.iznos_sa_pdv <= decimal.Parse(_configuration["MaxPonuda"]) || decimal.Parse(_configuration["MaxPonuda"]) == 0)
                    {
                        pon.status = "Z";

                        pon.radnik = User.Identity.Name;
                        _dbContext.SaveChanges();
                        return Ok(pon);
                    }
                    else
                        return Forbid();
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }
        }

        [HttpPut]
        [Route("kopirajPonudu")]
        public IActionResult KopirajPonudu(string broj)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    var newPonuda = new ponuda();
                    Helper.CopyPropertiesTo<ponuda, ponuda>(pon, newPonuda);

                    newPonuda.status = "E";
                    newPonuda.datum = DateTime.Now;

                    var ponude = _dbContext.ponuda.Where(p => p.datum.Year == DateTime.Now.Year);
                    string maxPonudaBroj = null;
                    int year = newPonuda.datum.Year;
                    int? brojPonude = null;
                    if (ponude != null && ponude.Count() > 0)
                    {
                        maxPonudaBroj = ponude.Max(p => p.broj);
                        int dbroj = int.Parse(maxPonudaBroj.Split("/")[0]);
                        brojPonude = dbroj + 1;
                    }
                    else
                        brojPonude = 1;

                    newPonuda.broj = brojPonude.Value.ToString("D5") + "/" + year.ToString(); ;

                    var stavke = _dbContext.ponuda_stavka.Where(s => s.ponuda_broj == pon.broj);

                    var stavkePonude = new List<ponuda_stavka>();
                    foreach (var stavka in stavke)
                    {
                        var newPonudaStavka = new ponuda_stavka();
                        Helper.CopyPropertiesTo<ponuda_stavka, ponuda_stavka>(stavka, newPonudaStavka);
                        newPonudaStavka.ponuda_broj = newPonuda.broj;
                        newPonudaStavka.ponuda = newPonuda;
                        stavkePonude.Add(newPonudaStavka);
                        //_dbContext.ponuda_stavka.Add(newPonudaStavka);
                    }
                    newPonuda.stavke = stavkePonude;

                    _dbContext.ponuda.Add(newPonuda);

                    _dbContext.SaveChanges();
                    return Ok(newPonuda);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

        }


        [HttpDelete]
        [Route("obrisiPonudu")]
        public IActionResult ObrisiPonudu(string broj)
        {
            var pon = _dbContext.ponuda.FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    _dbContext.Remove(pon);
                    _dbContext.SaveChanges();
                    return Ok(pon);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }
        }

        [HttpPut]
        [Route("statusiraj")]
        public IActionResult Statusiraj(string broj, string status)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    pon.status = status;
                    _dbContext.SaveChanges();
                    return Ok(pon);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }
        }

        [HttpPut]
        [Route("otkljucajPonudu")]
        public IActionResult OtkljucajPonudu(string broj)
        {
            var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
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
                    return BadRequest();
                }
            }
        }

        [HttpGet]
        public IEnumerable<ponuda> Get()
        {

            var ponude = _dbContext.ponuda.Include(p => p.stavke).Include(p => p.partner).Include(p => p.Korisnik).OrderByDescending(p => p.broj).ToList();
            return ponude;
        }



        [HttpGet]
        [Route("getbybroj")]
        public ponuda GetByBroj(string broj)
        {
            var ponuda = _dbContext.ponuda.Include(p => p.stavke).Include(p => p.Korisnik).Include(p => p.partner).FirstOrDefault(p => p.broj == broj);
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
                ponuda.radnik = User.Identity.Name;
                ponuda.status = "E";
                //ponuda.partner_sifra = 14;
                ponuda.broj = broj.Value.ToString("D5") + "/" + year.ToString();
                ponuda.iznos_bez_rabata = 0;
                ponuda.iznos_sa_pdv = 0;
                ponuda.iznos_sa_pdv = 0;
                ponuda.pdv = 0;
                ponuda.iznos_sa_rabatom = 0;
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
                ponuda.Korisnik = _dbContext.korisnik.FirstOrDefault(k => k.korisnicko_ime == ponuda.radnik);
                return Ok(ponuda);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
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
            ponuda.Korisnik = null;
            var pon = _dbContext.ponuda.FirstOrDefault(p => p.broj == ponuda.broj);
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
                catch (Exception ex)
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
            if (stavke != null && stavke.Count() > 0)
                ponuda_stavka = stavke.Max(ps => ps.stavka_broj);

            stavka.stavka_broj = ponuda_stavka == null ? 1 : (ponuda_stavka.Value + 1);
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
            var stavke = _dbContext.ponuda_stavka.Where(sp => sp.ponuda_broj == ponuda_broj).ToList();
            return stavke;

        }

        [HttpGet]
        [Route("/ponuda_dokument")]
        public IEnumerable<ponuda_dokument> GetDokument(string ponuda_broj)
        {
            var dokumenti = _dbContext.ponuda_dokument.Where(sp => sp.ponuda_broj == ponuda_broj).ToList();
            return dokumenti.ToList().WithoutDatas();
        }

        [HttpPost]
        [Route("upload_dokument")]
        public IActionResult UploadDokument(IFormFile blob, string broj)
        {
            string filePath = null;
            try
            {
                filePath = Path.Combine(_configuration["ContentPath"],
                   blob.FileName.Split(".")[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + blob.FileName.Split(".")[1]);

                var pon = _dbContext.ponuda.Include(p => p.partner).Include(p => p.Korisnik).Include(p => p.stavke).FirstOrDefault(p => p.broj == broj);
                if (pon == null)
                    return NotFound();
                else
                {
                    //using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //{
                    var ms = new MemoryStream();
                    blob.OpenReadStream().CopyTo(ms);
                    byte[] Value = ms.ToArray();
                    short noviBroj = 0;
                    var dokument = new ponuda_dokument() { ponuda_broj = broj, dokument = Value, naziv = blob.FileName, dokument_broj = noviBroj };
                    _dbContext.ponuda_dokument.Add(dokument);
                    _dbContext.SaveChanges();
                    //    await blob.CopyToAsync(fileStream);
                    //}

                    return Ok(dokument);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, filePath);
                return BadRequest();
            }
        }
    }
}
