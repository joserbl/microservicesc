using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo platformRepo, IMapper mapper)
        {
            _platformRepo = platformRepo; 
            _mapper = mapper;
        }


        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest getAllRequest, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _platformRepo.GetAllPlatforms();

            foreach(var plat in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }
             
            return Task.FromResult(response);
            
        }

    }
}