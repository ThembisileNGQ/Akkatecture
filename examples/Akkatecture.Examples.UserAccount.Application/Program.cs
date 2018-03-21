using System;
using Akka.Actor;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;

namespace Akkatecture.Examples.UserAccount.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("useraccount-example");

            var aggregateManager = system.ActorOf(Props.Create(() => new UserAccountAggregateManager()));

            var aggregateId = UserAccountId.New;
            var command = new CreateUserAccountCommand(aggregateId, "foo user");

            aggregateManager.Tell(command);

            while (true)
            {
                
            }
        }
    }
}
