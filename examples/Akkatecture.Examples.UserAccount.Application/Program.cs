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
            var system = ActorSystem.Create("useraccount-example");

            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            var aggregateId = UserAccountId.New;
            var command = new CreateUserAccountCommand(aggregateId, "Foo");

            aggregateManager.Tell(command);

            Console.ReadLine();
        }
    }
}
