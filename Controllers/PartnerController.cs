using System;
using System.Collections.Generic;
using System.Linq;
using Delos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PartnerController : ControllerBase
    {

        private DelosDbContext _dbContext;

        public PartnerController(DelosDbContext context, ILogger<PartnerController> logger)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<partner> Get()
        {

            var partneri = _dbContext.partner.ToList().OrderByDescending(p=>p.sifra);
            return partneri;
        
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<partner> Search(string naziv)
        {
            var partneri = _dbContext.partner.Where(p => p.naziv.ToLower().Contains(naziv.ToLower()));
            return partneri;

        }

        [HttpDelete]
        public IActionResult DeletePartner(int sifra)
        {
            var partner = _dbContext.partner.FirstOrDefault(p => p.sifra == sifra);
            if (partner != null)
            {
                _dbContext.partner.Remove(partner);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }


        [HttpPost]
        public IActionResult InsertPartner(partner partner)
        {
            try
            {
                //var partneri = _dbContext.partner
                //string maxPartnerBroj = null;
                //int? broj = null;
                //if (partneri != null && partneri.Count() > 0)
                //{
                //    maxPartnerBroj = partneri.Max(p => p.broj);
                //    int dbroj = int.Parse(maxPartnerBroj.Split("/")[0]);
                //    broj = dbroj + 1;
                //}
                //else
                //    broj = 1;
                _dbContext.partner.Add(partner);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult UpdatePartner(partner partner)
        {
            var pon = _dbContext.partner.FirstOrDefault(p => p.sifra == partner.sifra);
            if (pon == null)
                return NotFound();
            else
            {
                try
                {
                    Helper.CopyPropertiesTo<partner, partner>(partner, pon);
                    _dbContext.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

    }
}
