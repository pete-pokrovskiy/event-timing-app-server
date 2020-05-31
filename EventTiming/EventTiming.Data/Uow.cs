using EventTiming.Data.Repositories.ByEntity;
using EventTiming.Data.Repositories.ByEntity.Interfaces;
using System;
using System.Threading.Tasks;

namespace EventTiming.Data
{
    public class Uow : IUow, IDisposable
    {
        private bool _disposed;

        private readonly EventTimingDbContext _context;

        public IEventRepository EventRepository { get; }

        public Uow(EventTimingDbContext context)
        {
            _context = context;

            EventRepository = new EventRepository(_context);
        }

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }


    }
}
