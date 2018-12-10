using Akkatecture.Core;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Aggregates
{
    public interface ISnapshot : IVersionedType
    {
    }
    
    public interface IAggregateSnapshot<TAggregate, TIdentity> : ISnapshot
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}