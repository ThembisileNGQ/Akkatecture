using System;
using Akka.Actor;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;

namespace Akkatecture.Examples.UserAccount.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Create actor system
            var system = ActorSystem.Create("useraccount-example");

            //Create supervising aggregate manager for UserAccount aggregate root actors
            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            //Build create user account aggregate command
            var aggregateId = UserAccountId.New;
            var createUserAccountCommand = new CreateUserAccountCommand(aggregateId, "foo bar");
            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            aggregateManager.Tell(createUserAccountCommand);
            
            var changeNameCommand = new UserAccountChangeNameCommand(aggregateId, "foo bar baz");
            aggregateManager.Tell(changeNameCommand);
                        
            //block end of program
            Console.ReadLine();
        }

       
    }
}
