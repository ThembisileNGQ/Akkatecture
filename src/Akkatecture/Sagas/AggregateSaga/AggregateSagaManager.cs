using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator> : ReceiveActor, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>
        where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
    {
        protected ILoggingAdapter Logger { get; }
        public Expression<Func<TAggregateSaga>> SagaFactory { get; }
        protected TSagaLocator SagaLocator { get; }
        public AggregateSagaManagerSettings Settings { get; }

        protected AggregateSagaManager(Expression<Func<TAggregateSaga>> sagaFactory, bool autoSubscribe = true)
        {
            Logger = Context.GetLogger();

            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));

            SagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            SagaFactory = sagaFactory;
            Settings = new AggregateSagaManagerSettings(Context.System.Settings.Config);

            var sagaType = typeof(TAggregateSaga);

            if (autoSubscribe && Settings.AutoSubscribe)
            {
                var sagaHandlesSubscriptionTypes =
                    sagaType
                        .GetSagaEventSubscriptionTypes();

                foreach (var type in sagaHandlesSubscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
            }

            if (Settings.AutoSpawnOnReceive)
            {
                ReceiveAsync<IDomainEvent>(Handle);
            }
            
        }
        
        protected virtual Task Handle(IDomainEvent domainEvent)
        {
            var sagaId = SagaLocator.LocateSaga(domainEvent);
            var saga = FindOrSpawn(sagaId);
            saga.Tell(domainEvent,Sender);
            return Task.CompletedTask;
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
