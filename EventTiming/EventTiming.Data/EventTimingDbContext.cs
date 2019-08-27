using EventTiming.Data.Conventions;
using EventTiming.Domain;
using EventTiming.Domain.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventTiming.Data
{
    public class EventTimingDbContext : IdentityDbContext
    {
        public EventTimingDbContext() //: base(GetDefaultDbContextOptions())
        {

        }

        //private static DbContextOptions GetDefaultDbContextOptions()
        //{
        //    string connectionString = ""
        //    DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
        //    optionsBuilder.UseSqlServer();
        //    return optionsBuilder.Options;
        //}

        public EventTimingDbContext(DbContextOptions<EventTimingDbContext> options) : base(options)
        {

        }

        public EventTimingDbContext(string connectionString) : base(GetDbContextOptionsByConnectionString(connectionString))
        {

        }
        private static DbContextOptions GetDbContextOptionsByConnectionString(string connectionString)
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventTimingItem> EventTimingItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.SingularTableNameConvention();

            base.OnModelCreating(modelBuilder);
        }


        public override int SaveChanges()
        {
            AddTimeStamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimeStamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimeStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Проставим дату создания и обновления
        /// </summary>
        private void AddTimeStamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is IDateTracking && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    //заполняем дату создания, только если она еще не указана
                    if (((IDateTracking)entry.Entity).CreatedDate == DateTime.MinValue)
                        ((IDateTracking)entry.Entity).CreatedDate = DateTime.Now;
                }

                ((IDateTracking)entry.Entity).ModifiedDate = DateTime.Now;

            }
        }
    }
}
