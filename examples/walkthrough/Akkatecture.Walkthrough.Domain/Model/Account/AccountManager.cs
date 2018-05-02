using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Akkatecture.Walkthrough.Domain.Model.Account
{
    public class AccountManager : AggregateManager<Account, AccountId, Command<Account, AccountId>>
    {
    }
}
