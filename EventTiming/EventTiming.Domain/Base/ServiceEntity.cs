using System;
using System.Collections.Generic;
using System.Text;

namespace EventTiming.Domain.Base
{
    public class ServiceEntity : IDateTracking
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
