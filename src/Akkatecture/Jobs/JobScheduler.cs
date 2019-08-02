using System;
using Akka.Actor;
using Akka.Persistence;
using Akkatecture.Jobs.Commands;
using Akkatecture.Jobs.Events;

namespace Akkatecture.Jobs
{
    public class JobScheduler<TJob, TIdentity> : ReceivePersistentActor, IJobScheduler
        where TJob : IJob
        where TIdentity : IJobId
    {
        private readonly ICancelable _tickerTask;
        public SchedulerState<TJob, TIdentity> State { get; private set; }
        public JobSchedulerSettings Settings { get; }
        public override string PersistenceId { get; }

        public JobScheduler()
        {
            Settings = new JobSchedulerSettings(Context.System.Settings.Config);
            State = SchedulerState<TJob, TIdentity>.New;
            
            
            PersistenceId = Settings.PersistenceId;
            JournalPluginId = Settings.JournalPluginId;
            SnapshotPluginId = Settings.SnapshotPluginId;
            
            _tickerTask = 
                Context
                    .System
                    .Scheduler
                    .ScheduleTellRepeatedlyCancelable(Settings.TickInterval, Settings.TickInterval, Self, Tick<TJob,TIdentity>.Instance, ActorRefs.NoSender);

            Command<Tick<TJob,TIdentity>>(Execute);
            Command<Schedule<TJob, TIdentity>>(Execute);
            Command<Cancel<TJob, TIdentity>>(Execute);
            
            Recover<Scheduled<TJob, TIdentity>>(ApplySchedulerEvent);
            Recover<Cancelled<TJob, TIdentity>>(ApplySchedulerEvent);
            Recover<Finished<TJob, TIdentity>>(ApplySchedulerEvent);

        }

        private bool Execute(Tick<TJob, TIdentity> tick)
        {
            throw new NotImplementedException();
        }
        
        private bool Execute(Schedule<TJob, TIdentity> command)
        {
            throw new NotImplementedException();
        }
        
        private bool Execute(Cancel<TJob, TIdentity> command)
        {
            throw new NotImplementedException();
        }

        protected virtual bool ShouldTriggerSchedule(Schedule<TJob, TIdentity> schedule, DateTime now)
        {
            var shouldTrigger = schedule.TriggerDate <= now;
            return shouldTrigger;
        }

        public void ApplySchedulerEvent(SchedulerEvent<TJob, TIdentity> schedulerEvent)
        {
            switch (schedulerEvent)
            {
                case Scheduled<TJob, TIdentity> scheduled:
                    State = State.AddEntry(scheduled.Entry);
                    break;
                case Finished<TJob, TIdentity> completed:
                    State = State.RemoveEntry(completed.Id);
                    break;
                case Cancelled<TJob, TIdentity> cancelled:
                    State = State.RemoveEntry(cancelled.Id);
                    break;
                default: throw new ArgumentException(nameof(schedulerEvent));
            }
        }
        
        private void Emit<TEvent>(TEvent schedulerEvent, Action<TEvent> handler) where TEvent : SchedulerEvent<TJob, TIdentity>
        {
            PersistAsync(schedulerEvent, e =>
            {
                handler(e);
            });
        }
        
        protected override void PostStop()
        {
            _tickerTask.Cancel();
            base.PostStop();
        }
    }
}