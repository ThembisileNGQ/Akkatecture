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
            ProbeRef.Tell(TestJobDone.Instance);

            return true;
        }
    }

    public class TestJobDone
    {
        public static TestJobDone Instance => new TestJobDone();
    }
}