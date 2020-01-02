// The MIT License (MIT)
//
// Copyright (c) 2018 - 2020 Lutando Ngqakaza
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
using Akka.Cluster.Tools.Singleton;
using Akkatecture.Clustering.Dispatchers;
using Akkatecture.Subscribers;

namespace Akkatecture.Clustering.Core
{
    public static class SingletonFactory<TDomainEventSubscriber>
        where TDomainEventSubscriber : DomainEventSubscriber
    {
        public static IActorRef StartSingletonSubscriber(
            ActorSystem actorSystem,
            Expression<Func<TDomainEventSubscriber>> domainEventSubscriberFactory,
            string roleName)
        {
            var name = typeof(TDomainEventSubscriber).Name;

            var domainEventSubscriberProps = Props.Create(domainEventSubscriberFactory);
            
            actorSystem.ActorOf(ClusterSingletonManager.Props(
                    singletonProps: Props.Create(() => new ClusterParentProxy(domainEventSubscriberProps, true)),
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(actorSystem).WithRole(roleName).WithSingletonName(name)),
                name: name);

            var proxy = StartSingletonSubscriberProxy(actorSystem, roleName);

            actorSystem.ActorOf(Props.Create(() =>
                new SingletonDomainEventSubscriberDispatcher<TDomainEventSubscriber>(proxy)),$"{typeof(TDomainEventSubscriber).Name}Dispatcher");
                
            return proxy;
        }

        public static IActorRef StartSingletonSubscriberProxy(ActorSystem actorSystem, string roleName)
        {
            var name = typeof(TDomainEventSubscriber).Name;
            
            var proxy = actorSystem.ActorOf(ClusterSingletonProxy.Props(
                    singletonManagerPath: $"/user/{name}",
                    settings: ClusterSingletonProxySettings.Create(actorSystem).WithRole(roleName).WithSingletonName(name)),
                name: $"{name}Proxy");

            return proxy;
        }
    }
}