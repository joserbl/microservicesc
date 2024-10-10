using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository; 
            _mapper = mapper; 
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"INFO --> GetCommandsPlatform: {platformId}");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound(); 
            }
            
            var commandItems = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }


        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"INFO --> Hit GetCommandForPlatform: PLATFORM_ID: {platformId} / COMMAND_ID: {commandId}");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound(); 
            }

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
             Console.WriteLine($"INFO --> Hit CreateCommandForPlatform: PLATFORM_ID: {platformId}");

            if(!_repository.PlatformExists(platformId))
            {
                return NotFound(); 
            }

            var command = _mapper.Map<Command>(commandCreateDto);

            _repository.createCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new {platformId = platformId, CommandID = commandReadDto.Id}, commandReadDto);
        }




    }
}