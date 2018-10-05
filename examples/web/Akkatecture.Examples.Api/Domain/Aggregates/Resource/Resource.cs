using Akkatecture.Aggregates;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    public class Resource : AggregateRoot<Resource,ResourceId,ResourceState>
    {
        public Resource(ResourceId id)
            : base(id)
        {
        }
    }
}