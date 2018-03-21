using Akkatecture.Commands;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands
{
    public class CreateUserAccountCommand : Command<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public CreateUserAccountCommand(
            UserAccountId aggregateId,
            string name)
            : base(aggregateId)
        {
            Name = name;
        }
    }


    //not in use yet
    public class CreateUserAccountCommandHandler : CommandHandler<UserAccountAggregate, UserAccountId, CreateUserAccountCommand>
    {
        public override bool Handle(UserAccountAggregate aggregate, CreateUserAccountCommand command)
        {
            aggregate.Create(command.Name);
            return true;
        }
    }
}