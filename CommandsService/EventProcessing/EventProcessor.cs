using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory; 
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    break;
                default:
                    break;
            }
        }


        private EventType DetermineEvent(string notifMessage)
        {
            Console.WriteLine("INFO --> Determining Event...");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifMessage);

            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform published event detected!!!!");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Event type NOT detected!!!!");
                    return EventType.Undetermined;
            }

        }

        private void addPlatform(string PlatformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var PlatformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(PlatformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(PlatformPublishedDto);

                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else
                    {
                    Console.WriteLine($"Platform already exists...");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Couldn't add platform to DB!!!! -> {ex.Message}");
                }
            }
        }
    }


    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}