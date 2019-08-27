using Microsoft.AspNetCore.Identity;
using System;

namespace EventTiming.Domain.Base
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class Entity : IDateTracking, IUserTracking
    { 
        protected Entity()
        {
        }

        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IdentityUser CreatedBy { get; set; }
        //public Guid? CreatedById { get; set; }
        public IdentityUser ModifiedBy { get; set; }
        //public Guid? ModifiedById { get; set; }
    }
}
