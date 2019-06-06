using System;

namespace EventTiming.Domain.Base
{
    public interface IDateTracking
    {
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
    }
}
