using Akkatecture.Core;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    public class ResourceId : Identity<ResourceId>
    {
        public ResourceId(string value) 
            : base(value)
        {
        }
    }
}