using System;
using System.Collections.Generic;
using System.Linq;
using Delos.Contexts;
using Delos.Helpers;
using Delos.Model;
using Delos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class KorisnikController : ControllerBase
    {
        private IUserService _userService;
        private DelosDbContext _dbContext;


        private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(DelosDbContext context, ILogger<KorisnikController> logger, IUserService userService)
        {
            _dbContext = context;
            _userService = userService;
        }



        [HttpGet]
        public IEnumerable<korisnik> Get()
        {

            var korisnici = _userService.GetAll();
            return korisnici;

        }
        [HttpDelete]
        public IActionResult DeleteKorisnik(string korisnickoIme)
        {
            var korisnik = _dbContext.korisnik.FirstOrDefault(p => p.korisnicko_ime == korisnickoIme);
            if (korisnik != null)
            {
                _dbContext.korisnik.Remove(korisnik);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }

        [HttpPost]
        public IActionResult InsertKorisnik(korisnik korisnik)
        {
            try
            {
                korisnik.lozinka = Helper.CreateMD5(korisnik.lozinka).ToLower();
                //korisnik.role = new List<string>();
                _dbContext.korisnik.Add(korisnik);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("izmjenaLozinke")]
        public IActionResult IzmjenaLozinke(korisnik korisnik)
        {
            var pon = _dbContext.korisnik.FirstOrDefault(p => p.korisnicko_ime == korisnik.korisnicko_ime);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    var user = _userService.Authenticate(korisnik.korisnicko_ime, korisnik.lozinkaStara);

                    if (user == null)
                        return BadRequest(new { message = "Username or password is incorrect" });
                    pon.lozinka = Helper.CreateMD5(korisnik.lozinka);

                    _dbContext.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }
        [HttpPut]
        public IActionResult UpdateKorisnik(korisnik korisnik)
        {
            var pon = _dbContext.korisnik.FirstOrDefault(p => p.korisnicko_ime == korisnik.korisnicko_ime);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    pon.ime = korisnik.ime;
                    pon.prezime = korisnik.prezime;
                    pon.email = korisnik.email;
                    pon.admin = korisnik.admin;
                    pon.role = korisnik.role;
                    _dbContext.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
}
