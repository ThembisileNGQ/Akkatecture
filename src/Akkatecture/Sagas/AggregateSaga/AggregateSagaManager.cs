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
using System.Collections.Generic;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;
using Akkatecture.Messages;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator> : ReceiveActor, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>, new()
    {
        private IReadOnlyList<Type> _subscriptionTypes { get; }
        protected ILoggingAdapter Logger { get; }
        public Expression<Func<TAggregateSaga>> SagaFactory { get; }
        protected TSagaLocator SagaLocator { get; }
        public AggregateSagaManagerSettings Settings { get; }

        protected AggregateSagaManager(Expression<Func<TAggregateSaga>> sagaFactory)
        {
            Logger = Context.GetLogger();

            _subscriptionTypes = new List<Type>();

            SagaLocator = new TSagaLocator();
            SagaFactory = sagaFactory;
            Settings = new AggregateSagaManagerSettings(Context.System.Settings.Config);

            var sagaType = typeof(TAggregateSaga);

            if (Settings.AutoSubscribe)
            {
                var sagaEventSubscriptionTypes =
                    sagaType
                        .GetSagaEventSubscriptionTypes();

                var asyncSagaEventSubscriptionTypes =
                    sagaType
                        .GetAsyncSagaEventSubscriptionTypes();

                var subscriptionTypes = new List<Type>();
                
                subscriptionTypes.AddRange(sagaEventSubscriptionTypes);
                subscriptionTypes.AddRange(asyncSagaEventSubscriptionTypes);

                _subscriptionTypes = subscriptionTypes.AsReadOnly();
                
                foreach (var type in _subscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }

            }

            if (Settings.AutoSpawnOnReceive)
            {
                Receive<IDomainEvent>(Handle);
            }

            Receive<UnsubscribeFromAll>(Handle);
        }

        protected virtual bool Handle(UnsubscribeFromAll command)
        {
            UnsubscribeFromAllTopics();

            return true;
        }

        protected void UnsubscribeFromAllTopics()
        {
            foreach (var type in _subscriptionTypes)
            {
                Context.System.EventStream.Unsubscribe(Self, type);
            }
        }
        
        protected virtual bool Handle(IDomainEvent domainEvent)
        {
            var sagaId = SagaLocator.LocateSaga(domainEvent);
            var saga = FindOrSpawn(sagaId);
            saga.Tell(domainEvent,Sender);
            return true;
        }

        protected virtual bool Terminate(Terminated message)
        {
            Logger.Warning("AggregateSaga of Type={0}, and Id={1}; has terminated.",typeof(TAggregateSaga).PrettyPrint(), message.ActorRef.Path.Name);
            Context.Unwatch(message.ActorRef);
            return true;
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 3000,
                localOnlyDecider: x =>
                {
                    Logger.Warning("{0} will supervise Exception={1} to be decided as {2}.",
                        GetType().PrettyPrint(), x.ToString(),Directive.Restart);
                    return Directive.Restart;
                });
        }

        protected IActorRef FindOrSpawn(TIdentity sagaId)
        {
            var saga = Context.Child(sagaId);
            if (saga.IsNobody())
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
}
