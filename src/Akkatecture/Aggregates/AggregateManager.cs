using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateManager<TAggregate, TIdentity, TCommand> :  ReceiveActor, IAggregateManager<TAggregate, TIdentity>
        where TAggregate : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : class, ICommand<TAggregate,TIdentity>
    {
        protected ILoggingAdapter Logger { get; set; }
        protected Func<DeadLetter, bool> DeadLetterHandler => Handle;
        public AggregateManagerSettings Settings { get; }

        protected AggregateManager()
        {
            Logger = Context.GetLogger();
            Settings = new AggregateManagerSettings(Context.System.Settings.Config);
            
            Receive<TCommand>(Dispatch);
            Receive<Terminated>(Terminate);

            if (Settings.HandleDeadLetters)
            {
                Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));
                Receive<DeadLetter>(DeadLetterHandler);
            }
            
             
        }

        protected virtual bool Dispatch(TCommand command)
        {
            Logger.Info($"{GetType().PrettyPrint()} received {command.GetType().PrettyPrint()}");

            var aggregateRef = FindOrCreate(command.AggregateId);

            aggregateRef.Tell(command);

            return true;
        }

        
        protected virtual bool ReDispatch(TCommand command)
        {
            Logger.Info($"{GetType().PrettyPrint()} as dead letter {command.GetType().PrettyPrint()}");

            var aggregateRef = FindOrCreate(command.AggregateId);

            aggregateRef.Tell(command);

            return true;
        }

        protected bool Handle(DeadLetter deadLetter)
        {
            if (deadLetter.Message is TCommand &&
                (deadLetter.Message as TCommand).AggregateId.GetType() == typeof(TIdentity))
            {
                var command = deadLetter.Message as TCommand;

                ReDispatch(command);
            }

            return true;

        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning($"{typeof(TAggregate).PrettyPrint()}: {message.ActorRef.Path} has terminated.");
            Context.Unwatch(message.ActorRef);
            return true;
        }

        protected virtual IActorRef FindOrCreate(TIdentity aggregateId)
        {
            var aggregate = Context.Child(aggregateId);

            if (Equals(aggregate,ActorRefs.Nobody))
            {
                aggregate = CreateAggregate(aggregateId);      
            }

            return aggregate;
        }

        protected virtual IActorRef CreateAggregate(TIdentity aggregateId)
        {
            var aggregateRef = Context.ActorOf(Props.Create<TAggregate>(aggregateId),aggregateId.Value);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {

                    Logger.Warning($"[{GetType().PrettyPrint()}] Exception={x.ToString()} to be decided.");
                    return Directive.Restart;
                });
        }
        
    }
}