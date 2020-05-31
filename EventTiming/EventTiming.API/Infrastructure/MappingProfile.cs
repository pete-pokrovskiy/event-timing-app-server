using AutoMapper;
using EventTiming.API.Contract;
using EventTiming.Logic.Contract.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.API.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventInput, EventDto>();
        }
    }
}
