using Akkatecture.Aggregates;
using Akkatecture.Examples.Api.Domain.Aggregates.Resource.Commands;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    public class Resource : AggregateRoot<Resource,ResourceId,ResourceState>
    {
        public Resource(ResourceId id)
            : base(id)
        {
            var handler = new CreateResourceCommandHandler();
            Command<CreateResourceCommand>(x => handler.HandleCommand(this, Context, x));
        }
    }
}