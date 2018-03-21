using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Core.VersionedTypes;

namespace Akkatecture.Commands
{
    public interface ICommand : IVersionedType
    {
        //Task<ISourceId> PublishAsync(ICommandBus commandBus, CancellationToken cancellationToken);
        ISourceId GetSourceId();
    }

    public interface ICommand<in TAggregate, out TIdentity, out TSourceIdentity> : ICommand
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TSourceIdentity : ISourceId
    {
        TIdentity AggregateId { get; }
        TSourceIdentity SourceId { get; }
    }

    public interface ICommand<in TAggregate, out TIdentity> : ICommand<TAggregate, TIdentity, ISourceId>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}