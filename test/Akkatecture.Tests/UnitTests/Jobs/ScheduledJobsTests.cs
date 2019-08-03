using System;
using System.ComponentModel;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Akkatecture.Jobs.Commands;
using Akkatecture.Jobs.Responses;
using Akkatecture.TestHelpers.Jobs;
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
            var scheduler = (TestScheduler)Sys.Scheduler;
            var testJobScheduler = Sys.ActorOf(Props.Create(() => new TestJobScheduler()).WithDispatcher(CallingThreadDispatcher.Id), "test-job");
            var when = DateTime.UtcNow.AddDays(1);
            
            var job = new TestJob("hi");
            var schedule = new Schedule<TestJob, TestJobId>(jobId, probe.Ref.Path, job, when);
            
            testJobScheduler.Tell(schedule, probe);
            
            probe.ExpectMsg<ScheduleAddedSuccess<TestJobId>>(x => x.Id == jobId,TimeSpan.FromMinutes(1));
        }
        
    }
}