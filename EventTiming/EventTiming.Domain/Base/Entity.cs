using System;

namespace EventTiming.Domain.Base
{
    /// <summary>
    /// Base class for entitiese
    /// </summary>
    public abstract class Entity : IDateTracking, IUserTracking
    { 
        protected Entity()
        {
        }

        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ExternalId { get; set;}
        public User CreatedBy { get; set; }
        public Guid? CreatedById { get; set; }
        public User ModifiedBy { get; set; }
        public Guid? ModifiedById { get; set; }
    }
}
