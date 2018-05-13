using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Akka.Actor;
using Akkatecture.Walkthrough.Domain.Model.Account;
using Akkatecture.Walkthrough.Domain.Model.Account.Commands;
using Akkatecture.Walkthrough.Domain.Model.Account.Entities;
using Akkatecture.Walkthrough.Domain.Model.Account.ValueObjects;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.Queries;
using Akkatecture.Walkthrough.Domain.Repositories.Revenue.ReadModels;
using Akkatecture.Walkthrough.Domain.Sagas.MoneyTransfer;
using Akkatecture.Walkthrough.Domain.Subscribers;

namespace Akkatecture.Walkthrough.Application
{
    public class Program
    {
        public static IActorRef AccountManager { get; set; }
        public static IActorRef RevenueRepository { get; set; }
        public static void CreateActorSystem()
        {
            //Create actor system
            var system = ActorSystem.Create("bank-system");

            var aggregateManager = system.ActorOf(Props.Create(() => new AccountManager()),"account-manager");
            
            //Create revenue repository
            RevenueRepository = system.ActorOf(Props.Create(() => new RevenueRepository()),"revenue-repository");
            
            //Create subscriber for revenue
            system.ActorOf(Props.Create(() => new RevenueSubscriber(RevenueRepository)),"revenue-subscriber");
            
            //Create saga manager
            system.ActorOf(Props.Create(() =>
                new MoneyTransferSagaManager(() => new MoneyTransferSaga(aggregateManager))),"moneytransfer-saga");
            
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

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            Console.WriteLine("Walkthrough operations complete.\n\n");
            Console.WriteLine("Press Enter to get the revenue:");
            
            Console.ReadLine();

            var revenue = RevenueRepository.Ask<RevenueReadModel>(new GetRevenueQuery(), TimeSpan.FromMilliseconds(500)).Result;

            Console.WriteLine($"The Revenue is: {revenue.Revenue.Value}.");
            Console.WriteLine($"From: {revenue.Transactions} transaction(s).");
            
            Console.ReadLine();           
        }
    }
}
