using EventTiming.Data.Repositories.ByEntity.Interfaces;
using EventTiming.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTiming.Data.Repositories.ByEntity
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(DbContext context) : base(context)
        {
        }
    }
}
