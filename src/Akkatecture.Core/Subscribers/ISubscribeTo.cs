using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Subscribers
{
    public interface ISubscribeTo<TAggregate, in TIdentity, in TEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        Task Handle(IDomainEvent<TAggregate, TIdentity, TEvent> domainEvent);
    }
}