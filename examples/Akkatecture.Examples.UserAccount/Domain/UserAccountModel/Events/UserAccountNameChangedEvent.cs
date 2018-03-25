using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events
{
    [EventVersion("UserAccountNameChanged", 1)]
    public class UserAccountNameChangedEvent : AggregateEvent<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountNameChangedEvent(string name)
        {
            Name = name;
        }
    }
}