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
            var command = new CreateUserAccountCommand(aggregateId, "Foo");

            
            //Send command, this is equivalent to command.publish() in other cqrs frameworks
            aggregateManager.Tell(command);

            //send a new command the change the user account name
            var newCommand = new UserAccountChangeNameCommand(aggregateId, "Foo2");
            aggregateManager.Tell(newCommand);

            for (var i = 0; i < 8; i++)
            {
                var loopCommand = new UserAccountChangeNameCommand(aggregateId, $"Foo{i}");
                aggregateManager.Tell(loopCommand);
            }
            
            var newCommand2 = new UserAccountChangeNameCommand(aggregateId, "FooEnd");
            aggregateManager.Tell(newCommand2);
            //block end of program
            Console.ReadLine();
        }
    }
}
