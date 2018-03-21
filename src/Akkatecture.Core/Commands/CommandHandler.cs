using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Commands
{
    public abstract class CommandHandler<TAggregate, TIdentity,TResult,TCommand> :
        ICommandHandler<TAggregate, TIdentity,TResult, TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        public abstract TResult HandleCommand(
            TAggregate aggregate,
            TCommand command);
    }

    public abstract class CommandHandler<TAggregate, TIdentity, TCommand> :
        CommandHandler<TAggregate, TIdentity,bool,TCommand>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate, TIdentity>
    {
        public override bool HandleCommand(
            TAggregate aggregate,
            TCommand command)
        {
            Handle(aggregate, command);
            return true;
        }

        public abstract bool Handle(
            TAggregate aggregate,
            TCommand command);
    }
}