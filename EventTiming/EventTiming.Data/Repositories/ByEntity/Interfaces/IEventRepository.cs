using EventTiming.Data.Repositories.Interfaces;
using EventTiming.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventTiming.Data.Repositories.ByEntity.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
    }
}
