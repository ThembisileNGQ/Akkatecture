using Akkatecture.Core;

namespace Akkatecture.TestHelpers.Aggregates
{
    public class TestId : Identity<TestId> 
    {
        public TestId(string value)
            : base(value)
        {
            
        }
    }
}