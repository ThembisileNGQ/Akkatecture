using System;
using Akka.Actor;

namespace Akkatecture.Jobs.Commands
{
    
    public sealed class ScheduleRepeatedly<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TimeSpan Interval { get; }

        public ScheduleRepeatedly(
            TIdentity id,
            ActorPath jobRunner,
            TJob job,
            TimeSpan interval,
            DateTime triggerDate)
            : base(id, jobRunner, job, triggerDate)
        {
            Interval = interval;
        }
    }
}