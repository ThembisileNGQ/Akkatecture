using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.Messaging
{
    public abstract class Envelope<TAggregate,TIdentity,TCommand> : IEnvelope<TAggregate,TIdentity,TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate,TIdentity>
    {
        public TIdentity DestinationId { get; set; }
        public TCommand Command { get; }

        protected Envelope(TIdentity destinationId, TCommand command)
        {
            if (destinationId == null) throw new ArgumentNullException(nameof(destinationId));
            if (command == null) throw new ArgumentNullException(nameof(command));

            DestinationId = destinationId;
            Command = command;
        }

    }
}