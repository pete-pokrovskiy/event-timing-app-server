using EventTiming.Data.Repositories.Interfaces;
using EventTiming.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Data.Repositories.ByEntity.Interfaces
{
    public interface IEventTimingItemRepository : IGenericRepository<EventTimingItem>
    {
        
    }
}
