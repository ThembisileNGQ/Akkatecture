using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Sagas
{

    //rename aggregatesagamanager?
    public abstract class SagaManager<TAggregateSaga, TIdentity, TSagaLocator, TSagaState> : ReceiveActor
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : ISagaLocator<TIdentity>
        where TSagaState : SagaState<TAggregateSaga, TIdentity, IEventApplier<TAggregateSaga, TIdentity>>
        where TAggregateSaga : AggregateSaga<TAggregateSaga,TIdentity, TSagaState>
//Saga<TIdentity, SagaState<TAggregateSaga,TIdentity, IEventApplier<TAggregateSaga,TIdentity>>>
    {
        protected ILoggingAdapter Logger { get; set; }
        private readonly Expression<Func<TAggregateSaga>> SagaFactory;
        private TSagaLocator SagaLocator { get; }

        protected SagaManager(Expression<Func<TAggregateSaga>> sagaFactory)
        {
            Logger = Context.GetLogger();

            Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));

            SagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            SagaFactory = sagaFactory;
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

    public class FooSagaManager : SagaManager<FooSaga, FooSagaId, FooSagaLocator, FooSagaState>
    {
        public FooSagaManager(Expression<Func<FooSaga>> sagaFactory)
            : base(sagaFactory)
        {
            
        }
    }

    
    public class FooSaga : AggregateSaga<FooSaga,FooSagaId, FooSagaState>
    {
        public FooSaga(int i, string q, long j)
        {
            
        }
    }

    public class FooSagaLocator : ISagaLocator<FooSagaId>
    {
        public FooSagaId LocateSaga(IDomainEvent domainEvent)
        {
            var sagaId = domainEvent.GetIdentity();
            return new FooSagaId($"foosaga-{sagaId}");
        }
    }

    public class FooSagaId : SagaId<FooSagaId>
    {
        public FooSagaId(string value)
            : base(value)
        {
            
        }
    }

    public class FooSagaState : SagaState<FooSaga, FooSagaId, IEventApplier<FooSaga,FooSagaId>>
    {
        
    }
}
