using Akka.TestKit.Xunit2;
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
        
    }
}