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
            ProbeRef.Tell(TestJobDone.With(job.Greeting));

            return true;
        }
    }

    public class TestJobDone
    {
        public string Greeting { get; }
        public static TestJobDone With(string greeting) => new TestJobDone(greeting);
        public TestJobDone(
            string greeting)
        {
            Greeting = greeting;
        }
    }
}