using System;
using Akka.Actor;
using Akkatecture.ValueObjects;
using Cronos;

namespace Akkatecture.ScheduledJobs
{
    public class Schedule<TJob,TIdentity> : ValueObject
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TIdentity JobId { get; }
        public ActorPath JobRunner { get; }
        public TJob Job { get; }
        public DateTime TriggerDate  { get; }

        public Schedule(
            TIdentity jobId, 
            ActorPath jobRunner, 
            TJob job,
            DateTime triggerDate)
        {
            
            JobId = jobId;
            JobRunner = jobRunner;
            Job = job;
            TriggerDate = triggerDate;
        }

        public virtual Schedule<TJob,TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            return null;
        }
    }

    public class ScheduleRepeatedly<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public TimeSpan Interval { get; }

        public ScheduleRepeatedly(
            TIdentity jobId,
            ActorPath jobRunner,
            TJob job,
            TimeSpan interval,
            DateTime triggerDate)
            : base(jobId, jobRunner, job, triggerDate)
        {
            Interval = interval;
        }
    }
    
    public class ScheduleCron<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public string CronExpression { get; }
        private readonly CronExpression _expression;

        public ScheduleCron(
            TIdentity jobId,
            ActorPath jobRunner,
            TJob job,
            string cronExpression,
            DateTime triggerDate)
            : base(jobId, jobRunner, job, triggerDate)
        {
            CronExpression = cronExpression;
            _expression = Cronos.CronExpression.Parse(cronExpression);
        }
        
        public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            var next = _expression.GetNextOccurrence(utcDate);
            if (next.HasValue)
                return new ScheduleCron<TJob, TIdentity>(JobId, JobRunner, Job, CronExpression, next.Value);
            else
                return null;
        }
    }
    
}