using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

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

        public string AggregateShardResolver<TAggregate,TIdentity>(object message)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            if(message is null)
                throw new ArgumentNullException();

            if (message is ICommand<TAggregate, TIdentity> command)
                return Math.Abs(GetPersistenceHash(command.AggregateId.Value)).ToString();

            throw new ArgumentException(nameof(message));
            
        }
        
        public int GetPersistenceHash(string aggregateId)
        {
            var length = aggregateId.Length;
            return aggregateId[length - 3] * 100 +
                   aggregateId[length - 2] * 10 +
                   aggregateId[length - 1];
        }
    }
}