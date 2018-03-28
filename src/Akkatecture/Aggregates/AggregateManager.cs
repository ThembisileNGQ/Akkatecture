using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateManager<TAggregate, TIdentity, TCommand, TState> : ReceiveActor
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TState>
        where TState : AggregateState<TAggregate, TIdentity, IEventApplier<TAggregate, TIdentity>>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate,TIdentity>
    {
        protected ILoggingAdapter Logger { get; set; }

        protected AggregateManager()
        {
            Logger = Context.GetLogger();

            Become(Manager);
        }

        protected virtual void Manager()
        {
            Receive<TCommand>(Dispatch);
            Receive<Terminated>(Terminate);
            Receive<object>(Fail);
        }
        
        protected virtual bool Dispatch(TCommand command)
        {
            Logger.Info($"{GetType().DeclaringType} received {command.GetType()}");

            var aggregateRef = FindOrCreate(command.AggregateId);

            aggregateRef.Tell(command);

            return true;
        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning($"Aggregate Root: {message.ActorRef.Path} has terminated.");
            Context.Unwatch(message.ActorRef);
            return true;
        }

        protected virtual bool Fail(object message)
        {
            Logger.Warning($"{this.GetType().DeclaringType} received {message.GetType()}");
            Unhandled(message);
            return true;
        }

        protected virtual IActorRef FindOrCreate(TIdentity aggregateId)
        {
            var aggregateRef = Context.Child(aggregateId);

            if (Equals(aggregateRef,ActorRefs.Nobody))
            {
                aggregateRef = CreateAggregate(aggregateId);
            }

            Context.Watch(aggregateRef);

            return aggregateRef;
        }

        protected virtual IActorRef CreateAggregate(TIdentity aggregateId)
        {
            var aggregateRef = Context.ActorOf(Props.Create<TAggregate>(aggregateId),aggregateId.Value);

            return aggregateRef;
        }

        protected  override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {

                    Logger.Error($"[{GetType()}] Exception={x.ToString()} to be decided.");
                    return Directive.Restart;
                });
        }
    }
}