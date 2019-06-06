using System;

namespace EventTiming.Domain.Base
{
    /// <summary>
    /// Трэкинг юзера, создавшего/изменившего запись
    /// </summary>
    public interface IUserTracking
    {
        User CreatedBy { get; set; }
        Guid? CreatedById { get; set; }
        User ModifiedBy { get; set; }
        Guid? ModifiedById { get; set; }
    }
}
