using System;
using Akka.Actor;
using Cronos;

namespace Akkatecture.Jobs.Commands
{
    
    public sealed class ScheduleCron<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public string CronExpression { get; }
        private readonly CronExpression _expression;

        public ScheduleCron(
            TIdentity id,
            ActorPath jobRunner,
            TJob job,
            string cronExpression,
            DateTime triggerDate)
            : base(id, jobRunner, job, triggerDate)
        {
            CronExpression = cronExpression;
            _expression = Cronos.CronExpression.Parse(cronExpression);
        }
        
        public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            var next = _expression.GetNextOccurrence(utcDate);
            if (next.HasValue)
                return new ScheduleCron<TJob, TIdentity>(Id, JobRunner, Job, CronExpression, next.Value);
            
            return null;
        }
    }
}