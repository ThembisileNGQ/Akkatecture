using System;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Clustering.Core
{
    public class ClusterFactory
    {
        public IActorRef StartAggregateCluster(ActorSystem actorSystem, Type aggregateRootManager)
        {
            if (aggregateRootManager != typeof(AggregateManager<,,,>))
            {
                throw new ArgumentException(nameof(aggregateRootManager));
            }

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;



            throw new NotImplementedException();
        }

        public IActorRef StartAggregateCluster<TAggregateManager>(ActorSystem actorSystem)
        {
            if (typeof(TAggregateManager) != typeof(AggregateManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateManager).PrettyPrint()} is not {typeof(AggregateManager<,,,>).PrettyPrint()}");
            }

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            throw new NotImplementedException();
        }
        
        public IActorRef StartAggregateClusterProxy(ActorSystem actorSystem, Type aggregateRootManager)
        {
            throw new NotImplementedException();
        }

        public IActorRef StartAggregateSagaCluster(ActorSystem actorSystem, Type aggregateSagaManager)
        {

            throw new NotImplementedException();
        }
        
    }

    /*public class PoCClusterFactory<TAggregateManager, TAggregate, TIdentity>
        where TAggregateManager : AggregateManager<TAggregate, TIdentity, ICommand<TAggregate, TIdentity>, IEventApplier<TAggregate, TIdentity>>
        where TIdentity : IIdentity
        where TAggregate : AggregateRoot<TAggregate,TIdentity,IEventApplier<TAggregate,TIdentity>>
    {
        
    }*/
    
}
