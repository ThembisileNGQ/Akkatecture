using Akkatecture.Commands;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands
{
    public class UserAccountChangeNameCommand : Command<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountChangeNameCommand(UserAccountId aggreagateId, string name)
            : base(aggreagateId)
        {
            Name = name;
        }
    }
}