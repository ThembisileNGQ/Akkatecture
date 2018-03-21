using System;
using System.Collections.Generic;
using System.Text;
using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events
{
    [EventVersion("UserAccountCreated", 1)]
    public class UserAccountCreatedEvent : AggregateEvent<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountCreatedEvent(string name)
        {
            Name = name;
        }
    }
}
