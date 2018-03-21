using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Core;

namespace Akkatecture.Aggregates
{
    public interface IAggregateRoot
    {
        IAggregateName Name { get; }
        int Version { get; }
        //IEnumerable<IUncommittedEvent> UncommittedEvents { get; }
        bool IsNew { get; }

        //Task<IReadOnlyCollection<IDomainEvent>> CommitAsync(IEventStore eventStore, ISnapshotStore snapshotStore, ISourceId sourceId, CancellationToken cancellationToken);

        bool HasSourceId(ISourceId sourceId);

        void ApplyEvents(IEnumerable<IAggregateEvent> aggregateEvents);

        void ApplyEvents(IReadOnlyCollection<IDomainEvent> domainEvents);

        IIdentity GetIdentity();

        //Task LoadAsync(IEventStore eventStore, ISnapshotStore snapshotStore, CancellationToken cancellationToken);
    }

    public interface IAggregateRoot<out TIdentity> : IAggregateRoot
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}