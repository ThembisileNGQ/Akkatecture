using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
using Akkatecture.Clustering.Dispatchers;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Core
{
    public class ClusterFactory<TAggregateManager,TAggregate,TIdentity>
        where TAggregateManager : ActorBase, IAggregateManager<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public static IActorRef StartAggregateCluster(ActorSystem actorSystem, int numberOfShards = 12)
        { 
            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.Start(
                typeof(TAggregateManager).Name,
                Props.Create<TAggregateManager>(),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );

            return shardRef;
        }
        
        public static IActorRef StartAggregateClusterProxy(ActorSystem actorSystem, string clusterRoleName, int numberOfShards = 12)
        {
            var clusterSharding = ClusterSharding.Get(actorSystem);

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.StartProxy(
                typeof(TAggregateManager).Name,
                clusterRoleName,
                ShardIdentityExtractors
                    .AggregateIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );
            
            return shardRef;
        }
        
    }

    public class ClusterFactory<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSagaManager : ActorBase, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>
    {
        public static IActorRef StartAggregateSagaCluster(ActorSystem actorSystem, Expression<Func<TAggregateSaga>> sagaFactory, string clusterRoleName, int numberOfShards = 12)
        {
            if (sagaFactory == null)
            {
                throw new ArgumentNullException(nameof(sagaFactory));
            }

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.Start(
                typeof(TAggregateSagaManager).Name,
                Props.Create<TAggregateSagaManager>(sagaFactory,false),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateSagaIdentityExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>,
                shardResolver.AggregateSagaShardResolver<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
            );

            actorSystem.ActorOf(Props.Create(() =>
                new ShardedAggregateSagaDispatcher<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>(
                    clusterRoleName, numberOfShards)));

            return shardRef;
        }

        public static IActorRef StartAggregateSagaClusterProxy(ActorSystem actorSystem, string clusterRoleName, int numberOfShards = 12)
        {
            if (typeof(TAggregateSagaManager) != typeof(AggregateSagaManager<,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateSagaManager).PrettyPrint()} is not a {typeof(AggregateSagaManager<,,>).PrettyPrint()}");
            }
            var clusterSharding = ClusterSharding.Get(actorSystem);

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.StartProxy(
                typeof(TAggregateSagaManager).Name,
                clusterRoleName,
                ShardIdentityExtractors
                    .AggregateSagaIdentityExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>,
                shardResolver.AggregateSagaShardResolver<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
            );

            return shardRef;
        }

    }



}
