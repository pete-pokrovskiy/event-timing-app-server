using System;

namespace EventTiming.Domain.Base
{
    public class JoinEntity : IDateTracking, IUserTracking
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public User CreatedBy { get; set; }
        public Guid? CreatedById { get; set; }
        public User ModifiedBy { get; set; }
        public Guid? ModifiedById { get; set; }
    }
}
