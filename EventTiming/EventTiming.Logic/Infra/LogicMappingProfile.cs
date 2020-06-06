using AutoMapper;
using EventTiming.Domain;
using EventTiming.Logic.Contract.Dto;
using System.Collections.Generic;

namespace EventTiming.Logic.Infra
{
    public class LogicMappingProfile : Profile
    {
        public LogicMappingProfile()
        {
            CreateMap<EventTimingItem, EventTimingItemDto>().ReverseMap();
            //CreateMap<List<EventTimingItem>, List<EventTimingItemDto>>().ReverseMap();
        }
    }
}
