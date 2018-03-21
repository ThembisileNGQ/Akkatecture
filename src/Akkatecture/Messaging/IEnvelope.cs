using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.Messaging
{
    public interface IEnvelope<out TIdentity, out TCommand> //: IEnvelope
        where TIdentity : IIdentity
        where TCommand : ICommand
    {
        TIdentity DestinationId { get; }
        TCommand Command { get; }
    }

    public interface IEnvelope<TAggregate,out TIdentity,out TCommand> : IEnvelope<TIdentity, TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        
    }
}