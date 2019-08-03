using Akkatecture.Jobs;

namespace Akkatecture.TestHelpers.Jobs
{
    [JobName("test-job")]
    public class TestJob : IJob
    {
        public string Greeting { get; }
        public TestJob(string greeting)
        {
            Greeting = greeting;
        }
    }
}