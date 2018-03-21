using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Commands
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TAggregate, TIdentity,TResult, in TCommand> : ICommandHandler
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        TResult HandleCommand(TAggregate aggregate, TCommand command);
    }
}