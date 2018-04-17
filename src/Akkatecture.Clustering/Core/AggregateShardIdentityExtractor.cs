using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.Clustering.Core
{
    internal class AggregateShardIdentityExtractor
    {
        public static Tuple<string, object> IdentityExtrator<TAggregate,TIdentity>(object message)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            if(message is null)
                throw new ArgumentNullException();
            
            if (message is ICommand<TAggregate, TIdentity> command)
                return new Tuple<string, object>(command.AggregateId.Value, message);

            throw new ArgumentException(nameof(message));
        }
    }
    
}