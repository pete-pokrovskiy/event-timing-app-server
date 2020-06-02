﻿using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Infra;
using System;

namespace EventTiming.Logic.Contract.Events
{
    public class CreateTimingItemCommand : Command
    {

        public Guid EventId { get; set; }
        public EventTimingItemDto TimingItem { get; set; }
    }
}
