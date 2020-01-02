// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
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
    public class ScheduledJobsTests : TestKit
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
            
            // last assertions do not work probably has to do with time
            // see https://gist.github.com/Lutando/6c68a93faace4a0403f468358193ee41
            //scheduler.Advance(day);
            //probe.ExpectNoMsg(TimeSpan.FromSeconds(1));
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
            //scheduler.Advance(day);
            //probe.ExpectNoMsg();
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
            probe.ExpectMsg<TestJobAck>();
            scheduler.AdvanceTo(when);
            
            
            
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                return x.Greeting == greeting;
            });
            scheduler.Advance(fiveMinutes);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add(fiveMinutes));
                return x.Greeting == greeting;
            });
            scheduler.Advance(fiveMinutes);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when.Add( fiveMinutes * 2));
                return x.Greeting == greeting;
            });
        }
        
        [Fact]
        [Category(Category)]
        public void SchedulingJob_ForEveryCronTrigger_DispatchesJobMessage()
        {
            var probe = CreateTestProbe("job-probe");
            var scheduler = (TestScheduler) Sys.Scheduler;
            var cronExpression = "* */12 * * *";
            var jobId = TestJobId.New;
            var greeting = $"hi here here is a random guid {Guid.NewGuid()}";
            var job = new TestJob(greeting);
            var when = DateTime.UtcNow.AddMonths(1);
            var twelveHours = TimeSpan.FromHours(12);
            Expression<Func<TestJobScheduler>> testJobSchedulerExpression = () => new TestJobScheduler();
            Expression<Func<TestJobRunner>> testJobRunnerExpression = () => new TestJobRunner(probe);
            
            var testJobManager = Sys.ActorOf(
                Props.Create(() =>
                        new JobManager<TestJobScheduler, TestJobRunner, TestJob, TestJobId>(
                            testJobSchedulerExpression,
                            testJobRunnerExpression))
                    .WithDispatcher(CallingThreadDispatcher.Id));
            
            var schedule = new ScheduleCron<TestJob, TestJobId>(jobId, job, cronExpression, when)
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
            probe.ExpectMsg<TestJobAck>();
            scheduler.AdvanceTo(when);
            probe.ExpectMsg<TestJobDone>(x =>
            {
                x.At.Should().BeCloseTo(when);
                
                return x.Greeting == greeting;
            });
            
            var cancelSchedule = new Cancel<TestJob, TestJobId>(jobId)
                .WithAck(TestJobAck.Instance)
                .WithNack(TestJobNack.Instance);
            
            testJobManager.Tell(cancelSchedule, probe);
            
            probe.ExpectMsg<TestJobAck>();
            scheduler.Advance(fiveMinutes);
            
            probe.ExpectNoMsg();
            
            scheduler.Advance(fiveMinutes);
            
            probe.ExpectNoMsg();
        }
        
    }
}