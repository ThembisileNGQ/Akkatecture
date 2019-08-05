using System;
using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Akkatecture.Jobs.Commands;
using Akkatecture.Jobs.Responses;
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
            
            var probe = CreateTestProbe("job-handler");
            var jobId = TestJobId.New;
            var scheduler = (TestScheduler) Sys.Scheduler;
            var greeting = "Hi from the past";
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddDays(1);
            
            var job = new TestJob(greeting);
            var schedule = new Schedule<TestJob, TestJobId>(jobId, probe.Ref.Path, job, when);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<ScheduleAddedSuccess<TestJobId>>(x => x.JobId == jobId);
            
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJob>(x => x.Greeting == greeting);
        }
        
        [Fact]
        [Category(Category)]
        public void SchedulingJob_ForEvery5minutes_DispatchesJobMessage()
        {
            
            var probe = CreateTestProbe("job-handler");
            var jobRunner = Sys.ActorOf(Props.Create(() => new TestJobRunner(probe.Ref)));
            var jobId = TestJobId.New;
            var scheduler = (TestScheduler) Sys.Scheduler;
            var greeting = "Hi from the past";
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddDays(1);
            
            var job = new TestJob(greeting);
            var schedule = new ScheduleRepeatedly<TestJob, TestJobId>(jobId, jobRunner.Path, job, TimeSpan.FromMinutes(5), when);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<ScheduleAddedSuccess<TestJobId>>(x => x.JobId == jobId);
            
            scheduler.AdvanceTo(when);

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
            
            scheduler.Advance(TimeSpan.FromMinutes(5));

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
            
            scheduler.Advance(TimeSpan.FromMinutes(5));    

            probe.ExpectMsg<TestJobDone>(x => x.Greeting == greeting);
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
            var schedule = new ScheduleCron<TestJob, TestJobId>(jobId, jobRunner.Path, job, cronExpression, when);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<ScheduleAddedSuccess<TestJobId>>(x => x.JobId == jobId);
            
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
        
    }
}