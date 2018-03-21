using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Commands
{
    public abstract class DistinctCommand<TAggregate, TIdentity> : ICommand<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly Lazy<ISourceId> _lazySourceId;

        public ISourceId SourceId => _lazySourceId.Value;
        public TIdentity AggregateId { get; }

        protected DistinctCommand(
            TIdentity aggregateId)
        {
            if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));

            _lazySourceId = new Lazy<ISourceId>(CalculateSourceId, LazyThreadSafetyMode.PublicationOnly);

            AggregateId = aggregateId;
        }

        private CommandId CalculateSourceId()
        {
            var bytes = GetSourceIdComponents().SelectMany(b => b).ToArray();
            return CommandId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Commands,
                bytes);
        }

        protected abstract IEnumerable<byte[]> GetSourceIdComponents();

        
        public ISourceId GetSourceId()
        {
            return SourceId;
        }
    }
}