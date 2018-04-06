using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.TestHelpers.Aggregates.Entities
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class TestId : Identity<TestId>
    {
        public TestId(string entityId)
            : base(entityId)
        {
            
        }
    }
}