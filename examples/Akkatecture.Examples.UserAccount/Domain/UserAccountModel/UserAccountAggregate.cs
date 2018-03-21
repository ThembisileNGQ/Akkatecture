using System;
using Akka.Persistence;
using Akkatecture.Aggregates;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel
{
    public class UserAccountAggregate : AggregateRoot<UserAccountAggregate,UserAccountId,UserAccountState>
    {
        public UserAccountAggregate(UserAccountId id)
            : base(id)
        {
            Register<UserAccountCreatedEvent>(Apply);
            Become(UserAccount);
        }


        public void UserAccount()
        {
            //
            Command<CreateUserAccountCommand>(Handle);


            //recovery from persistence
            Recover<UserAccountCreatedEvent>(Recover);
            Recover<SnapshotOffer>(Recover);
        }

        public bool Handle(CreateUserAccountCommand command)
        {
            Create(command.Name);
            return true;
        }

        public void Create(string name)
        {
            if (Version <= 0)
            {
                Emit(new UserAccountCreatedEvent(name));
            }        
        }

        public void Apply(UserAccountCreatedEvent aggregateEvent)
        {
            State.Apply(aggregateEvent);
        }
        
    }
}