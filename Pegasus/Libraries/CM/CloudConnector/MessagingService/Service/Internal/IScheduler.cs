using System;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A basic interface to allow a scheduler component to control when next work is performed
    /// </summary>
    internal interface IScheduler
    {
        Int32 GetTimeToNextWork(Boolean someWorkDone);
    }
}
