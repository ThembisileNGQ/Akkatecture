using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.TestHelpers.Aggregates
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class TestAggregateId : Identity<TestAggregateId> 
    {
        public TestAggregateId(string value)
            : base(value)
        {
            
        }
    }
}