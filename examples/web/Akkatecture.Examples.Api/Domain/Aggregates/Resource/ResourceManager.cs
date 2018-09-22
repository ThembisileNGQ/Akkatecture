using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.Examples.Api.Domain.Aggregates.Resource
{
    public class ResourceManager : AggregateManager<Resource,ResourceId,Command<Resource,ResourceId>>
    {
        
    }
}