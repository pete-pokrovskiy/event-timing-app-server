using Microsoft.AspNetCore.Identity;
using System;

namespace EventTiming.Domain.Base
{
    public class JoinEntity : IDateTracking, IUserTracking
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IdentityUser CreatedBy { get; set; }
        public Guid? CreatedById { get; set; }
        public IdentityUser ModifiedBy { get; set; }
        public Guid? ModifiedById { get; set; }
    }
}
