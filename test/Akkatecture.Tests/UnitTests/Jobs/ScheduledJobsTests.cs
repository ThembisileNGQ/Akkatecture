using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Akkatecture.Jobs;
using Akkatecture.Jobs.Commands;
using Akkatecture.TestHelpers.Jobs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Akkatecture.Tests.UnitTests.Jobs
{
    [Collection("ScheduledJobsTests")]
    public class ScheduledJobsTests: TestKit
    {
        private const string Category = "Jobs";

        public ScheduledJobsTests(ITestOutputHelper testOutputHelper)
            : base(TestHelpers.Akka.Configuration.ConfigWithTestScheduler, "jobs-tests", testOutputHelper)
        {
            
        }

        [Fact]
        [Category(Category)]
        public void SchedulingJob_For5minutes_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var scheduler = (TestScheduler) Sys.Scheduler;
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(greeting);
            var when = DateTime.UtcNow.AddDays(1);
            Expression<Func<TestJobScheduler>> testJobSchedulerExpression = () => new TestJobScheduler();
            Expression<Func<TestJobRunner>> testJobRunnerExpression = () => new TestJobRunner(probe);
            
            var testJobManager = Sys.ActorOf(
                Props.Create(() =>
                new JobManager<TestJobScheduler, TestJobRunner, TestJob, TestJobId>(
                    testJobSchedulerExpression,
                    testJobRunnerExpression))
                .WithDispatcher(CallingThreadDispatcher.Id));
            
            var schedule = new Schedule<TestJob, TestJobId>(jobId, job, when)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            
            
            testJobManager.Tell(schedule, probe);
            probe.ExpectMsg<TestJobAck>();
            scheduler.AdvanceTo(when);

            
            
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });
            scheduler.AdvanceTo(when.AddDays(1));
            probe.ExpectNoMsg();
        }
        
        [Fact]
        [Category(Category)]
        public void SchedulingJob_ForEvery5minutes_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var scheduler = (TestScheduler) Sys.Scheduler;
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(greeting);
            var when = DateTime.UtcNow.AddDays(1);
            var fiveMinutes = TimeSpan.FromMinutes(5);
            Expression<Func<TestJobScheduler>> testJobSchedulerExpression = () => new TestJobScheduler();
            Expression<Func<TestJobRunner>> testJobRunnerExpression = () => new TestJobRunner(probe);
            
            var testJobManager = Sys.ActorOf(
                Props.Create(() =>
                        new JobManager<TestJobScheduler, TestJobRunner, TestJob, TestJobId>(
                            testJobSchedulerExpression,
                            testJobRunnerExpression))
                    .WithDispatcher(CallingThreadDispatcher.Id));
            
            var schedule = new ScheduleRepeatedly<TestJob, TestJobId>(jobId, job, fiveMinutes, when)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            
            
            testJobManager.Tell(schedule, probe);
            probe.ExpectMsg<TestJobAck>(TimeSpan.FromMinutes(1));
            scheduler.AdvanceTo(when);
            
            
            
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            }, TimeSpan.FromMinutes(1));
            scheduler.Advance(fiveMinutes);
            var t = this.Sys.Settings.System.Scheduler.Now.DateTime;
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(fiveMinutes));
                return x.Greeting == greeting;
            }, TimeSpan.FromMinutes(1));
            /*scheduler.Advance(fiveMinutes);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });*/
            /*var probe = CreateTestProbe("job-handler");
            var jobRunner = Sys.ActorOf(Props.Create(() => new TestJobRunner(probe.Ref)));
            var jobId = TestJobId.New;
            var scheduler = (TestScheduler) Sys.Scheduler;
            var greeting = "Hi from the past";
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddDays(1);
            
            var job = new TestJob(greeting);
            var schedule = new ScheduleRepeatedly<TestJob, TestJobId>(jobId, job, TimeSpan.FromMinutes(5), when)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<TestJobAck>();
            
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
            
            scheduler.Advance(TimeSpan.FromMinutes(5));

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
            
            scheduler.Advance(TimeSpan.FromMinutes(5));    

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);*/
        }
        
        [Fact]
        [Category(Category)]
        public void SchedulingJob_ForEveryCronTrigger_DispatchesJobMessage()
        {
            
            var probe = CreateTestProbe("job-handler");
            var cronExpression = "* */12 * * *";
            var jobRunner = Sys.ActorOf(Props.Create(() => new TestJobRunner(probe.Ref)));
            var jobId = TestJobId.New;
            var scheduler = (TestScheduler) Sys.Scheduler;
            var greeting = "Hi from the past";
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddMonths(1);
            
            var job = new TestJob(greeting);
            var schedule = new ScheduleCron<TestJob, TestJobId>(jobId, job, cronExpression, when)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<TestJobAck>();
            
            scheduler.AdvanceTo(when);
            
            var twelveHours = TimeSpan.FromHours(12);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                
                return x.Greeting == greeting;
            });
            
            scheduler.Advance(twelveHours);

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(twelveHours));
                
                return x.Greeting == greeting;
            });
            
            scheduler.Advance(twelveHours);    

            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(twelveHours * 2));
                
                return x.Greeting == greeting;
            });
        }
        
        
        [Fact]
        [Category(Category)]
        public void SchedulingJob_AndThenCancellingJob_CeasesDispatching()
        {
            
            var probe = CreateTestProbe("job-handler");
            var jobRunner = Sys.ActorOf(Props.Create(() => new TestJobRunner(probe.Ref)));
            var jobId = TestJobId.New;
            var scheduler = (TestScheduler) Sys.Scheduler;
            var greeting = "Hi from the past";
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddDays(1);
            
            var job = new TestJob(greeting);
            var schedule = new ScheduleRepeatedly<TestJob, TestJobId>(jobId, job, TimeSpan.FromMinutes(5), when)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<TestJobAck>();
            
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
            
            var cancelSchedule = new Cancel<TestJob, TestJobId>(jobId)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            testJobScheduler.Tell(cancelSchedule, probe);
            
            probe.ExpectMsg<TestJobAck>();
            
            scheduler.Advance(TimeSpan.FromMinutes(5));
            
            probe.ExpectNoMsg();
            
            scheduler.Advance(TimeSpan.FromMinutes(5));
            
            probe.ExpectNoMsg();
            
            
        }
        
    }
}