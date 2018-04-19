using System;
using Akkatecture.Aggregates;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Sagas;
using Akkatecture.Sagas.AggregateSaga;

namespace Akkatecture.Clustering.Core
{
    public class ShardIdentityExtractors
    {
        public static  Tuple<string, object> AggregateIdentityExtractor<TAggregate,TIdentity>(object message)
            where TIdentity : IIdentity
            where TAggregate : IAggregateRoot<TIdentity>
        {
            if(message is null)
                throw new ArgumentNullException();
            
            if (message is ICommand<TAggregate, TIdentity> command)
                return new Tuple<string, object>(command.AggregateId.Value, message);

            throw new ArgumentException(nameof(message));
        }

        public static Tuple<string, object> AggregateSagaIdentityExtractor<TAggregateSagaManager, TAggregateSaga, TIdentity, TSagaLocator>(object message)
            where TAggregateSagaManager : IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator>
            where TAggregateSaga : IAggregateSaga<TIdentity>
            where TIdentity : SagaId<TIdentity>
            where TSagaLocator : ISagaLocator<TIdentity>
        {
            if (message is null)
                throw new ArgumentNullException();

            var sagaLocator = (TSagaLocator)Activator.CreateInstance(typeof(TSagaLocator));

            if (message is IDomainEvent domainEvent)
                return new Tuple<string, object>(sagaLocator.LocateSaga(domainEvent).Value, message);

            throw new ArgumentException(nameof(message));
        }

    }
    
}