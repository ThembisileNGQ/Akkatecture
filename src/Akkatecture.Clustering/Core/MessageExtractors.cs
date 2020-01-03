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
using Akka.Cluster.Sharding;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Core
{
    public class MessageExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator> : HashCodeMessageExtractor
        where TAggregateSagaManager : IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
        where TAggregateSaga : IAggregateSaga<TIdentity>
        where TIdentity : SagaId<TIdentity>
        where TSagaLocator : class, ISagaLocator<TIdentity>, new()
    {
        private TSagaLocator SagaLocator { get; }
        public MessageExtractor(int maxNumberOfShards) 
            : base(maxNumberOfShards)
        {
            SagaLocator = new TSagaLocator();
        }

        public override string EntityId(object message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));


            if (message is IDomainEvent domainEvent)
                return SagaLocator.LocateSaga(domainEvent).Value;

            throw new ArgumentException(nameof(message));
        }
    }
    public class MessageExtractor<TAggregate, TIdentity> : HashCodeMessageExtractor
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public MessageExtractor(int maxNumberOfShards) 
            : base(maxNumberOfShards)
        {
        }

        public override string EntityId(object message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message is ICommand<TAggregate, TIdentity> command)
                return command.AggregateId.Value;

            throw new ArgumentException(nameof(message));
        }
    }
}