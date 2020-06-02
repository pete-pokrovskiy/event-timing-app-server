using EventTiming.Data.Repositories.ByEntity.Interfaces;
using EventTiming.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTiming.Data.Repositories.ByEntity
{
    public class EventTimingItemRepository : GenericRepository<EventTimingItem>, IEventTimingItemRepository
    {
        public EventTimingItemRepository(DbContext context) : base(context)
        {
        }
    }
}
