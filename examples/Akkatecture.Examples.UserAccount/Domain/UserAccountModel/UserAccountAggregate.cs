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
            //event handler registration
            Register<UserAccountCreatedEvent>(State.Apply);
            
            
            Become(UserAccount);
        }


        public void UserAccount()
        {
            //command handler registration
            Command<CreateUserAccountCommand>(Handle);


            //recovery from persistent event source
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
        
    }
}