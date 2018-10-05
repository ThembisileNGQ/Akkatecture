// The MIT License (MIT)
//
// Copyright (c) 2018 Lutando Ngqakaza
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
using Akkatecture.Aggregates;
using Akkatecture.Akka;
using Akkatecture.Configuration.DependancyInjection;
using Akkatecture.Clustering.Core;
using Akkatecture.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AkkatectureServiceCollectionExtensions
    {
        public static IAkkatectureBuilder AddAggregateManagerCluster<TAggregateManager, TAggregate, TIdentity>(
            this IAkkatectureBuilder builder,
            int numberOfShards = 12)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = ClusterFactory<TAggregateManager, TAggregate, TIdentity>
                .StartClusteredAggregate(
                    builder.ActorSystem,
                    numberOfShards);
            
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }
        
        public static IAkkatectureBuilder AddAggregateManagerCluster<TAggregateManager, TAggregate, TIdentity>(
            this IAkkatectureBuilder builder,
            Expression<Func<TAggregateManager>> aggregateManagerFactory,
            int numberOfShards = 12)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = ClusterFactory<TAggregateManager, TAggregate, TIdentity>
                .StartClusteredAggregate(
                    builder.ActorSystem,
                    numberOfShards);
            
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }
        
        public static IAkkatectureBuilder AddAggregateManagerClusterProxy<TAggregateManager, TAggregate, TIdentity>(
            this IAkkatectureBuilder builder,
            string clusterRoleName,
            int numberOfShards = 12)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = ClusterFactory<TAggregateManager, TAggregate, TIdentity>
                .StartAggregateClusterProxy(
                    builder.ActorSystem,
                    clusterRoleName,
                    numberOfShards);
            
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            builder.Services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return builder;
        }
    }
}