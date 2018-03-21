using Akkatecture.Core;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Aggregates
{
    public interface IAggregateEvent : IVersionedType
    {
    }

    public interface IAggregateEvent<TAggregate, TIdentity> : IAggregateEvent
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}