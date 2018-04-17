using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;

namespace Akkatecture.Clustering.Core
{
    public class ShardIdentityExtractors
    {
        public static Tuple<string, object> IdentityExtrator(object message)
        {
            if(message is null)
                throw new ArgumentNullException();
            
            if (message is IIdentity command)
                return new Tuple<string, object>(command.Value, message);

            throw new ArgumentException(nameof(message));
        }
        
        public static  Tuple<string, object> AggregateCommandIdentityExtractor<TAggregate,TIdentity>(object message)
            where TIdentity : IIdentity
            where TAggregate : IAggregateRoot<TIdentity>
        {
            if(message is null)
                throw new ArgumentNullException();
            
            if (message is ICommand<TAggregate, TIdentity> command)
                return new Tuple<string, object>(command.AggregateId.Value, message);

            throw new ArgumentException(nameof(message));
        }
        
        
        
    }
    
}