using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
      
        public PlatformsController()
        {
             
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("hdhduhdudh");

            return Ok("Inbound test from Platforms Controller");
        }

        
 
    }
}