// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Extensions;
using Akkatecture.Jobs.Commands;
using Akkatecture.Jobs.Events;
using Akkatecture.Jobs.Responses;

namespace Akkatecture.Jobs
{
    public class JobScheduler<TJob, TIdentity> : ReceivePersistentActor, IJobScheduler
        where TJob : IJob
        where TIdentity : IJobId
    {
        private readonly ICancelable _tickerTask;
        private static readonly IJobName JobName = typeof(TJob).GetJobName();
        public IJobName Name => JobName;
        private readonly IJobDefinitionService _jobDefinitionService;
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

            _jobDefinitionService = new JobDefinitionService(Context.GetLogger());
            
            Command<Tick<TJob,TIdentity>>(Execute);
            Command<Schedule<TJob, TIdentity>>(Execute);
            Command<Cancel<TJob, TIdentity>>(Execute);
            
            Recover<Scheduled<TJob, TIdentity>>(ApplySchedulerEvent);
            Recover<Cancelled<TJob, TIdentity>>(ApplySchedulerEvent);
            Recover<Finished<TJob, TIdentity>>(ApplySchedulerEvent);
            Recover<SnapshotOffer>(Recover);
            
            Command<SaveSnapshotSuccess>(Execute);
            Command<SaveSnapshotFailure>(Execute);
            Command<DeleteMessagesSuccess>(Execute);
            Command<DeleteMessagesFailure>(Execute);

        }

        private bool Execute(Tick<TJob, TIdentity> tick)
        {
            var now = Context.System.Scheduler.Now.UtcDateTime;
            
            foreach (var schedule in State.Entries.Values.Where(e => ShouldTriggerSchedule(e, now)))
            {
                _jobDefinitionService.Load(schedule.Job.GetType());
                var jobDefinition = _jobDefinitionService.GetDefinition(schedule.Job.GetType());
                Log.Info("JobScheduler of Name={0}, sending job of Definition={1} to JobRunner at Path={2}.", Name, jobDefinition, schedule.JobRunner);

                var selection = Context.ActorSelection(schedule.JobRunner);
                selection.Tell(schedule.Job, ActorRefs.NoSender);

                Emit(new Finished<TJob, TIdentity>(schedule.Id, now), ApplySchedulerEvent);

                var next = schedule.WithNextTriggerDate(now);
                if (next != null)
                {
                    Emit(new Scheduled<TJob, TIdentity>(next), ApplySchedulerEvent);
                }
            }

            return true;
        }
        
        private bool Execute(Schedule<TJob, TIdentity> command)
        {
            var sender = Sender;
            _jobDefinitionService.Load(command.Job.GetType());
            var jobDefinition = _jobDefinitionService.GetDefinition(command.Job.GetType());
            try
            {
                Emit(new Scheduled<TJob, TIdentity>(command), e =>
                {
                    ApplySchedulerEvent(e);
                    
                    if (!sender.IsNobody())
                    {
                        sender.Tell(new ScheduleAddedSuccess<TIdentity>(e.Entry.Id));
                    }
                    
                    Log.Info("JobScheduler for Job of Name={0}, Definition={1}, and Id={2}, has been successfully scheduled to run at TriggerDate={3}.", Name, jobDefinition, e.Entry.Id, e.Entry.TriggerDate);
                });
            }
            catch (Exception error)
            {
                if (!sender.IsNobody())
                {
                    sender.Tell(new ScheduleAddedFailed<TIdentity>(new List<string>{error.Message}));
                }
                
                Log.Error(error,"JobScheduler for Job of Name={0}, and Definition={1}, has failed to schedule a job.", Name, jobDefinition);
            }

            return true;
        }
        
        private bool Execute(Cancel<TJob, TIdentity> command)
        {
            var sender = Sender;
            var id = command.Id;
            var now = Context.System.Scheduler.Now.UtcDateTime;
            try
            {
                Emit(new Cancelled<TJob, TIdentity>(command.Id, now), e =>
                {
                    ApplySchedulerEvent(e);
                    if (!sender.IsNobody())
                    {
                        sender.Tell(new ScheduleAddedSuccess<TIdentity>(e.Id));
                    }
                    Log.Info("JobScheduler for Job of Name={0}, and Id={1}, has been successfully cancelled.", Name, e.Id);
                });
            }
            catch (Exception error)
            {
                if (!sender.IsNobody())
                {
                    sender.Tell(new ScheduleAddedFailed<TIdentity>(new List<string>{error.Message}));
                }
                
                Log.Error(error,"JobScheduler for Job of Name={0}, and Id={1} has failed to cancel.", Name, id);
            }

            return true;
        }

        private bool Execute(SaveSnapshotSuccess command)
        {
            Log.Debug("JobScheduler for Job of Name={0} Successfully saved its scheduler snapshot. Removing all events before SequenceNr={1}.", Name ,command.Metadata.SequenceNr);
            DeleteMessages(command.Metadata.SequenceNr - 1);

            return true;
        }
        
        private bool Execute(SaveSnapshotFailure command)
        {
            Log.Error(command.Cause, "Job of Name={0} at SequenceNr={1}, Failed to save scheduler snapshot.", Name, command.Metadata.SequenceNr);

            return true;
        }
        
        private bool Execute(DeleteMessagesSuccess command)
        {
            Log.Debug( "Job of Name={0} at SequenceNr={1}, deleted scheduler messages to SequenceNr={2}.", Name, LastSequenceNr, command.ToSequenceNr);

            return true;
        }
        
        private bool Execute(DeleteMessagesFailure command)
        {
            Log.Error(command.Cause, "Job of Name={0} at SequenceNr={1}, Failed to delete scheduler messages to SequenceNr={2}.", Name, LastSequenceNr, command.ToSequenceNr);

            return true;
        }

        private bool Recover(SnapshotOffer offer)
        {
            if (offer.Snapshot is SchedulerState<TJob, TIdentity> state)
            {
                State = state;
            }
            return true;
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
            PersistAsync(schedulerEvent, handler);
        }
        
        protected override void PostStop()
        {
            _tickerTask.Cancel();
            base.PostStop();
        }
    }
}