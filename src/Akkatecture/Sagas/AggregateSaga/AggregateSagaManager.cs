using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator, TSagaState> : ReceiveActor
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : ISagaLocator<TIdentity>
        where TSagaState : SagaState<TAggregateSaga, TIdentity, IEventApplier<TAggregateSaga, TIdentity>>
        where TAggregateSaga : AggregateSaga<TAggregateSaga, TIdentity, TSagaState>
    {
        protected ILoggingAdapter Logger { get; set; }
        private readonly Expression<Func<TAggregateSaga>> SagaFactory;
        protected TSagaLocator SagaLocator { get; }

        protected AggregateSagaManager(Expression<Func<TAggregateSaga>> sagaFactory, bool autoSubscribe = true)
        {
            Logger = Context.GetLogger();

            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));

            SagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            SagaFactory = sagaFactory;

            if (autoSubscribe)
            {
                var startedBySubscriptionTypes =
                    GetType()
                        .GetSagaStartEventSubscriptionTypes();

                foreach (var type in startedBySubscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }

                var sagaHandlesSubscriptionTypes =
                    GetType()
                        .GetSagaHandleEventSubscriptionTypes();

                foreach (var type in sagaHandlesSubscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
            }
            
        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning($"{GetType().PrettyPrint()}: {message.ActorRef.Path} has terminated.");
            Context.Unwatch(message.ActorRef);
            return true;
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {

                    Logger.Error($"[{GetType().PrettyPrint()}] Exception={x.ToString()} to be decided.");
                    return Directive.Restart;
                });
        }

        protected IActorRef FindOrSpawn(TIdentity sagaId)
        {
            var saga = Context.Child(sagaId);
            if (Equals(saga, ActorRefs.Nobody))
            {
                return Spawn(sagaId);
            }
            return saga;
        }

        private IActorRef Spawn(TIdentity sagaId)
        {
            var saga = Context.ActorOf(Props.Create(SagaFactory), sagaId.Value);
            Context.Watch(saga);
            return saga;
        }
        
    }
    
}
