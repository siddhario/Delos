﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PonudaController : ControllerBase
    {

        private BloggingContext _dbContext;

 
        private readonly ILogger<PonudaController> _logger;

        public PonudaController(BloggingContext context, ILogger<PonudaController> logger)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public IEnumerable<ponuda> Get()
        {

            var ponude = _dbContext.ponuda.ToList();
            return ponude;
        
        }
    }
}
