using System;
using System.Linq.Expressions;
using System.Reflection;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
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
            /*var t = typeof(TAggregateManager).BaseType;
            var t43 = typeof(TAggregateManager).BaseType == typeof(IAggregateManager<TAggregate, TIdentity>);
            //var t3 = t.GetGenericTypeDefinition();
            var t2 = typeof(TAggregateManager).GetGenericTypeDefinition() == typeof(AggregateManager<,,,>);
            if (typeof(TAggregateManager) != typeof(IAggregateManager<TAggregate,TIdentity>))
            {
                throw new ArgumentException($"{typeof(TAggregateManager).PrettyPrint()} is not a {typeof(AggregateManager<,,,>).PrettyPrint()}");
            }*/

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.Start(
                typeof(TAggregate).Name,
                Props.Create<TAggregateManager>(),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );

            return shardRef;
        }
        
        public static IActorRef StartAggregateClusterProxy(ActorSystem actorSystem, string roleName, int numberOfShards = 12)
        {
            /*if (typeof(TAggregateManager) != typeof(AggregateManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateManager).PrettyPrint()} is not a {typeof(AggregateManager<,,,>).PrettyPrint()}");
            }*/
            var clusterSharding = ClusterSharding.Get(actorSystem);

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.StartProxy(
                typeof(TAggregate).Name,
                roleName,
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
        where TSagaLocator : ISagaLocator<TIdentity>
    {
        public static IActorRef StartAggregateSagaCluster(ActorSystem actorSystem, Expression<Func<TAggregateSaga>> sagaFactory, int numberOfShards = 12)
        {
            if (typeof(TAggregateSagaManager) != typeof(AggregateSagaManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateSagaManager).PrettyPrint()} is not a {typeof(AggregateSagaManager<,,,>).PrettyPrint()}");
            }

            if (sagaFactory == null)
            {
                throw new ArgumentNullException(nameof(sagaFactory));
            }

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.Start(
                typeof(TAggregateSagaManager).Name,
                Props.Create<TAggregateSagaManager>(sagaFactory),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateSagaIdentityExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>,
                shardResolver.AggregateSagaShardResolver<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
            );

            return shardRef;
        }

        public static IActorRef StartAggregateClusterProxy(ActorSystem actorSystem, string roleName, int numberOfShards = 12)
        {
            if (typeof(TAggregateSagaManager) != typeof(AggregateSagaManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateSagaManager).PrettyPrint()} is not a {typeof(AggregateSagaManager<,,,>).PrettyPrint()}");
            }
            var clusterSharding = ClusterSharding.Get(actorSystem);

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.StartProxy(
                typeof(TAggregateSagaManager).Name,
                roleName,
                ShardIdentityExtractors
                    .AggregateSagaIdentityExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>,
                shardResolver.AggregateSagaShardResolver<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>
            );

            return shardRef;
        }

    }



}
