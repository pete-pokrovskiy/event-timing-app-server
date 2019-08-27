using Microsoft.AspNetCore.Identity;
using System;

namespace EventTiming.Domain.Base
{
    /// <summary>
    /// Трэкинг юзера, создавшего/изменившего запись
    /// </summary>
    public interface IUserTracking
    {
        IdentityUser CreatedBy { get; set; }
        //Guid? CreatedById { get; set; }
        IdentityUser ModifiedBy { get; set; }
        //Guid? ModifiedById { get; set; }
    }
}
