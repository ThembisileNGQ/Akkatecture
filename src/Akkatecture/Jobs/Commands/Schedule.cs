using System;
using Akka.Actor;
using Akkatecture.ValueObjects;

namespace Akkatecture.Jobs.Commands
{
    public class Schedule<TJob,TIdentity> : SchedulerCommand<TJob,TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public ActorPath JobRunner { get; }
        public TJob Job { get; }
        public DateTime TriggerDate  { get; }

        public Schedule(
            TIdentity id, 
            ActorPath jobRunner, 
            TJob job,
            DateTime triggerDate)
            : base(id)
        {
            JobRunner = jobRunner;
            Job = job;
            TriggerDate = triggerDate;
        }

        public virtual Schedule<TJob,TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            return null;
        }
    }
}