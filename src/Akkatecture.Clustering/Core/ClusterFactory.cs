using System;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;

namespace Akkatecture.Clustering.Core
{
    public class ClusterFactory<TAggregateManager,TAggregate,TIdentity>
        where TAggregateManager : ActorBase, IAggregateManager<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public static IActorRef StartAggregateCluster(ActorSystem actorSystem)
        {
            if (typeof(TAggregateManager) != typeof(AggregateManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateManager).PrettyPrint()} is not a {typeof(AggregateManager<,,,>).PrettyPrint()}");
            }

            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(10);

            var shardRef = clusterSharding.Start(
                typeof(TAggregate).Name,
                Props.Create<TAggregateManager>(),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateCommandIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );

            return shardRef;
        }
        
        public IActorRef StartAggregateClusterProxy(ActorSystem actorSystem, string roleName)
        {
            if (typeof(TAggregateManager) != typeof(AggregateManager<,,,>))
            {
                throw new ArgumentException($"{typeof(TAggregateManager).PrettyPrint()} is not a {typeof(AggregateManager<,,,>).PrettyPrint()}");
            }
            var clusterSharding = ClusterSharding.Get(actorSystem);

            var shardResolver = new ShardResolvers(10);

            var shardRef = clusterSharding.StartProxy(
                typeof(TAggregate).Name,
                roleName,
                ShardIdentityExtractors
                    .AggregateCommandIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );
            
            return shardRef;
        }

    }

}
