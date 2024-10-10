using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper)
        {
             _repository = repository; 
             _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("INFO --> Getting Platforms from COMMAND_SERVICE...");

            var platformItems = _repository.GetAllPlatforms(); 
            
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
            
        }


        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("Inbound test from Platforms Controller");

            return Ok("Inbound test from Platforms Controller");
        }

        
 
    }
}