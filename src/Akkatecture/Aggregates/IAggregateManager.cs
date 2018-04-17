using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface IAggregateManager
    {
        
    }

    public interface IAggregateManager<TAggregate, TIdentity> : IAggregateManager
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
}