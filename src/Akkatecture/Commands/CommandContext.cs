using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Google.Protobuf;

namespace Akkatecture.Commands
{
    internal sealed class CommandContext<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        //public Dictionary<IActorRef, ICommand<TAggregate,TIdentity>> SenderContexts { get; }
        private Dictionary<ISourceId, ICommand<TAggregate,TIdentity>> Commands { get; }
        private Dictionary<ISourceId, HandleStatus> Messages { get; }

        internal CommandContext()
        {
            Messages = new Dictionary<ISourceId, HandleStatus>();
            Commands = new Dictionary<ISourceId, ICommand<TAggregate,TIdentity>>();
        }

        internal void RegisterNew(ICommand<TAggregate, TIdentity> command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));
            
            //PinnedCommand = command;
            Commands[command.SourceId] = command;
            Messages[command.SourceId] = HandleStatus.PostReceive;
        }

        internal void SetHandled(ISourceId sourceId)
        {
            Messages[sourceId] = HandleStatus.PostHandle;
        }
        
        internal void SetApplied(ISourceId sourceId)
        {
            Messages[sourceId] = HandleStatus.PostApply;
        }

        internal void SetFinished(ISourceId sourceId)
        {
            var messageRemoved = Messages.Remove(sourceId);
            var commandsRemoved = Commands.Remove(sourceId);
        }
    }

    internal enum HandleStatus
    {
        PostReceive = 1,
        PostHandle = 2,
        PostApply = 3,
    }
}