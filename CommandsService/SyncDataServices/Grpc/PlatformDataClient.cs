using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

#pragma warning disable CS8604 // Possible null reference argument.

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper; 
            _config = configuration;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"INFO --> Calling gRPC Service {_config["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try 
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);          
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't retrieve information from gRPC server: {ex.Message}");
                return null; 
            }
        }
    }
}