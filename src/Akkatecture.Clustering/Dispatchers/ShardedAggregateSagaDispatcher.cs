using Akka.Actor;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.Clustering.Core;
using Akkatecture.Clustering.Extentions;
using Akkatecture.Extensions;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Dispatchers
{
    public class ShardedAggregateSagaDispatcher<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator> : ReceiveActor
        where TAggregateSagaManager : ActorBase, IAggregateSagaManager<TAggregateSaga,TIdentity,TSagaLocator>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>
        where TAggregateSaga : IAggregateSaga<TIdentity>
    {
        public IActorRef AggregateSagaManager { get; }
        private ILoggingAdapter Logger { get; }
        public ShardedAggregateSagaDispatcher(string proxyRoleName, int numberOfShards)
        {
            Logger = Context.GetLogger();

            AggregateSagaManager =
                ClusterFactory<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
                    .StartAggregateSagaClusterProxy(Context.System, proxyRoleName, numberOfShards);

            var sagaType = typeof(TAggregateSaga);

            var sagaHandlesSubscriptionTypes =
                sagaType
                    .GetSagaEventSubscriptionTypes();

            foreach (var type in sagaHandlesSubscriptionTypes)
            {
                Context.System.EventStream.Subscribe(Self, type);
            }
            
        }

        protected virtual bool Dispatch(IDomainEvent domainEvent)
        {
            AggregateSagaManager.Tell(domainEvent);

            Logger.Debug($"{GetType().PrettyPrint()} just dispatched {domainEvent.GetType().PrettyPrint()} to {AggregateSagaManager}");
            return true;
        }
        
    }
}
