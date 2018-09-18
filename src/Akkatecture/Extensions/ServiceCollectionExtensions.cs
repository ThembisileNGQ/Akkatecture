using System;
using System.Linq.Expressions;
using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Akka;
using Akkatecture.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Akkatecture.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddActorSystem(
            this IServiceCollection services,
            ActorSystem actorSystem)
        {
            services.AddSingleton<ActorSystem>(actorSystem);
            return services;
        }
        
        public static IServiceCollection AddAggregateManager<TAggregateManager, TAggregate, TIdentity>(
            this IServiceCollection services, ActorSystem actorSystem)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = actorSystem.ActorOf(Props.Create<TAggregateManager>());
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return services;
        }
        
        public static IServiceCollection AddAggregateManager<TAggregateManager, TAggregate, TIdentity>(
            this IServiceCollection services, ActorSystem actorSystem, Expression<Func<TAggregateManager>> aggregateManagerFactory)
            where TAggregateManager : ReceiveActor, IAggregateManager<TAggregate, TIdentity>
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateManager = actorSystem.ActorOf(Props.Create(aggregateManagerFactory));
            var actorRef = new ActorRefOfT<TAggregateManager>(aggregateManager);

            services.AddSingleton<IActorRef<TAggregateManager>>(actorRef);
            return services;
        }
    }
}