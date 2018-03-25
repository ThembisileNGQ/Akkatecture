using System.Collections.Generic;
using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface IAggregateRoot
    {
        IAggregateName Name { get; }
        
        long Version { get; }
        
        bool IsNew { get; }
        
        bool HasSourceId(ISourceId sourceId);

        void ApplyEvents(IEnumerable<IAggregateEvent> aggregateEvents);

        void ApplyEvents(IReadOnlyCollection<IDomainEvent> domainEvents);

        IIdentity GetIdentity();
    }

    public interface IAggregateRoot<out TIdentity> : IAggregateRoot
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}