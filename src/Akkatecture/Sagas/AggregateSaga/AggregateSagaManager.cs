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
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Extensions;

namespace Akkatecture.Sagas.AggregateSaga
{
    public abstract class AggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator> : ReceiveActor, IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>
        where TAggregateSaga : ReceivePersistentActor, IAggregateSaga<TIdentity>
    {
        protected ILoggingAdapter Logger { get; }
        public Expression<Func<TAggregateSaga>> SagaFactory { get; }
        protected TSagaLocator SagaLocator { get; }
        public AggregateSagaManagerSettings Settings { get; }

        protected AggregateSagaManager(Expression<Func<TAggregateSaga>> sagaFactory, bool autoSubscribe = true)
        {
            Logger = Context.GetLogger();


            SagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            SagaFactory = sagaFactory;
            Settings = new AggregateSagaManagerSettings(Context.System.Settings.Config);

            var sagaType = typeof(TAggregateSaga);

            if (autoSubscribe && Settings.AutoSubscribe)
            {
                var sagaEventSubscriptionTypes =
                    sagaType
                        .GetSagaEventSubscriptionTypes();

                foreach (var type in sagaEventSubscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
                
                var asyncSagaEventSubscriptionTypes =
                    sagaType
                        .GetAsyncSagaEventSubscriptionTypes();

                foreach (var type in asyncSagaEventSubscriptionTypes)
                {
                    Context.System.EventStream.Subscribe(Self, type);
                }
            }

            if (Settings.AutoSpawnOnReceive)
            {
                Receive<IDomainEvent>(Handle);
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
            Logger.Warning($"{GetType().PrettyPrint()}: {message.ActorRef.Path} has terminated.");
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

                    Logger.Error($"[{GetType().PrettyPrint()}] Exception={x.ToString()} to be decided.");
                    return Directive.Restart;
                });
        }

        protected IActorRef FindOrSpawn(TIdentity sagaId)
        {
            var saga = Context.Child(sagaId);
            if (Equals(saga, ActorRefs.Nobody))
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
