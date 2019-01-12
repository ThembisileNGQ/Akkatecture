// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        public static IActorRef StartClusteredAggregate(
            ActorSystem actorSystem,
            int numberOfShards = 12)
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
        
        public static IActorRef StartClusteredAggregate(
            ActorSystem actorSystem,
            Expression<Func<TAggregateManager>> aggregateManagerFactory,
            int numberOfShards = 12)
        { 
            var clusterSharding = ClusterSharding.Get(actorSystem);
            var clusterShardingSettings = clusterSharding.Settings;

            var shardResolver = new ShardResolvers(numberOfShards);

            var shardRef = clusterSharding.Start(
                typeof(TAggregateManager).Name,
                Props.Create<TAggregateManager>(aggregateManagerFactory),
                clusterShardingSettings,
                ShardIdentityExtractors
                    .AggregateIdentityExtractor<TAggregate, TIdentity>,
                shardResolver.AggregateShardResolver<TAggregate, TIdentity>
            );

            return shardRef;
        }
        
        public static IActorRef StartAggregateClusterProxy(
            ActorSystem actorSystem,
            string clusterRoleName,
            int numberOfShards = 12)
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
        public static IActorRef StartClusteredAggregateSaga(
            ActorSystem actorSystem,
            Expression<Func<TAggregateSaga>> sagaFactory,
            string clusterRoleName,
            int numberOfShards = 12)
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

        public static IActorRef StartAggregateSagaClusterProxy(
            ActorSystem actorSystem,
            string clusterRoleName,
            int numberOfShards = 12)
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
