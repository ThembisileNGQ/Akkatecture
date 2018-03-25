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
            Become(UserAccount);
        }


        public void UserAccount()
        {
            //command handler registration
            Command<CreateUserAccountCommand>(Handle);
            Command<UserAccountChangeNameCommand>(Handle);
            
            //recovery from persistent event source
            Recover<UserAccountCreatedEvent>(Recover);
            Recover<UserAccountNameChangedEvent>(Recover);
            Recover<SnapshotOffer>(Recover);
        }

        public bool Handle(CreateUserAccountCommand command)
        {
            Create(command.Name);
            return true;
        }

        public bool Handle(UserAccountChangeNameCommand command)
        {
            ChangeName(command.Name);
            return true;
        }

        private void Create(string name)
        {
            if (Version <= 0)
            {
                Emit(new UserAccountCreatedEvent(name));
            }
            else
            {
                //signal domain error
            }
        }

        private void ChangeName(string name)
        {
            if (Version >= 0)
            {
                Emit(new UserAccountNameChangedEvent(name));
            }
            else
            {
                //signal domain error
            }
        }
        
    }
}