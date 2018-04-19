using Akkatecture.Aggregates;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel
{
    public class UserAccountState : AggregateState<UserAccountAggregate,UserAccountId>,
        IApply<UserAccountCreatedEvent>,
        IApply<UserAccountNameChangedEvent>
    {
        public string Name { get; private set; }

        public void Apply(UserAccountCreatedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }

        public void Apply(UserAccountNameChangedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }

    }
}