using System.Collections.Generic;
using Delos.Contexts;
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

        public KorisnikController(DelosDbContext context, ILogger<KorisnikController> logger,IUserService userService)
        {
            _dbContext = context;
            _userService=userService;
        }



        [HttpGet]
        public IEnumerable<korisnik> Get()
        {

            var korisnici = _userService.GetAll();
            return korisnici;
        
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
