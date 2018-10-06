using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class ResourceId : Identity<ResourceId>
    {
        public ResourceId(string value) 
            : base(value)
        {
        }
    }
}