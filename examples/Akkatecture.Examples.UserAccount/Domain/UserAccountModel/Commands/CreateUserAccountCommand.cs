using Akkatecture.Commands;

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
    
}