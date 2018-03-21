using Akka.Actor;
using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel
{
    public class UserAccountAggregateManager : AggregateManager<UserAccountAggregate,UserAccountId, Command<UserAccountAggregate, UserAccountId>, UserAccountState>
    {
        
    }
}