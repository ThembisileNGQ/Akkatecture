using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.TestFixtures.Aggregates
{
    public interface IFixtureExecutor<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IFixtureAsserter<TAggregate, TIdentity> When(ICommand<TAggregate, TIdentity> command);
    }
}