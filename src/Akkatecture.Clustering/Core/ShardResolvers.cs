using System;
using Akka.Util;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Core
{
    public class ShardResolvers
    {
        private readonly int _shardsCount;
        public int NumberOfShards => _shardsCount > 0 ? _shardsCount : 12;

        public ShardResolvers(int shardsCount)
        {
            _shardsCount = shardsCount;
        }

        public string AggregateShardResolver<TAggregate, TIdentity>(object message)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            if (message is null)
                throw new ArgumentNullException();

            if (message is ICommand<TAggregate, TIdentity> command)
                return Math.Abs(GetPersistenceHash(command.AggregateId.Value) % NumberOfShards).ToString();

            throw new ArgumentException(nameof(message));

        }

        public string AggregateSagaShardResolver<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>(object message)
            where TAggregateSagaManager : IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
            where TAggregateSaga : IAggregateSaga<TIdentity>
            where TIdentity : SagaId<TIdentity>
            where TSagaLocator : ISagaLocator<TIdentity>
        {
            if (message is null)
                throw new ArgumentNullException();

            var sagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            if (message is IDomainEvent domainEvent)
                return Math.Abs(GetPersistenceHash(sagaLocator.LocateSaga(domainEvent).Value) % NumberOfShards).ToString();

            throw new ArgumentException(nameof(message));

        }
        
        public int GetPersistenceHash(string aggregateId)
        {
            return MurmurHash.StringHash(aggregateId);
        }
    }
}