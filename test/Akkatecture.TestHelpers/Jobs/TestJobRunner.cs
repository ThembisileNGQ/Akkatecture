using System;
using Akka.Actor;
using Akkatecture.Jobs;

namespace Akkatecture.TestHelpers.Jobs
{
    public class TestJobRunner : JobRunner<TestJob, TestJobId>,
        IRun<TestJob>
    {
        public IActorRef ProbeRef { get; }

        public TestJobRunner(
            IActorRef probeRef)
        {
            ProbeRef = probeRef;
        }

        public bool Run(TestJob job)
        {
            var time = Context.System.Settings.System.Scheduler.Now.DateTime;
            ProbeRef.Tell(new TestJobDone(job.Greeting, time));

            return true;
        }
    }

    public class TestJobDone
    {
        public string Greeting { get; }
        public DateTime At { get; }
        public TestJobDone(
            string greeting,
            DateTime at)
        {
            Greeting = greeting;
            At = at;
        }
    }
}