using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Croc.CFB.Logic.Services
{
    public interface IDelayedMailProcessingParams
    {
        int SendBatchSize { get;}
        int SendIterationsTimeout { get; }
        bool SendEmailsInErrorState { get;}
    }
}
