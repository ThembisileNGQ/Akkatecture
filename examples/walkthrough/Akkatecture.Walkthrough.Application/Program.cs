using System;
using System.Security.Cryptography.X509Certificates;
using Akka.Actor;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;
using Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer;
using Akkatecture.Walkthrough.Domain.Subscribers;

namespace Akkatecture.Walkthrough.Application
{
    public class Program
    {
        public static IActorRef AccountManager { get; set; }
        public static void CreateActorSystem()
        {
            //Create actor system
            var system = ActorSystem.Create("bank-system");

            var aggregateManager = system.ActorOf(Props.Create(() => new AccountManager()));
            
            //Create subscriber for revenue
            system.ActorOf(Props.Create(() => new RevenueSubscriber()));
            
            //Create saga manager
            system.ActorOf(Props.Create(() =>
                new MoneyTransferSagaManager(() => new MoneyTransferSaga(aggregateManager))));
            
            AccountManager = aggregateManager;
        }
        
        public static void Main(string[] args)
        {
            CreateActorSystem();
            
            var senderId = AccountId.New;
            var receiverId = AccountId.New;

            var senderOpeningBalance = new Money(509.23m);
            var receiverOpeningBalance = new Money(30.45m);
            
            var openSenderAccountCommand = new OpenNewAccountCommand(senderId, senderOpeningBalance);
            var openReceiverAccountCommand = new OpenNewAccountCommand(receiverId, receiverOpeningBalance);
            
            AccountManager.Tell(openReceiverAccountCommand);
            AccountManager.Tell(openSenderAccountCommand);
            
            var amountToSend = new Money(125.23m);
            var transaction = new Transaction(senderId, receiverId, amountToSend);
            var transferMoneyCommand = new TransferMoneyCommand(senderId,transaction);
            
            AccountManager.Tell(transferMoneyCommand);

            Console.ReadLine();
        }
    }
}
